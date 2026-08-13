using DbGovernator.Abstractions;
using DbGovernator.Realisations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.NDbClasses
{
    /// <summary>
    /// Реализует абстрактный класс DbCommand, является обёрткой для сех других классов, реализующих DbCommand. 
    /// Совместимость обёртки с PostgreSql гаранитруется.
    /// </summary>
    public class NDbCommand : DbCommand
    {
        /// <summary>
        /// Оборачиваемая команда.
        /// </summary>
        private DbCommand _innerCommand;

        /// <summary>
        /// Списки шагов выполнения, задаются в конструкторе.
        /// </summary>
        private List<ExecutionCommandStep> _executionSteps;
        /// <summary>
        /// Список посетителей, получается через DI.
        /// </summary>
        private List<IVisitor> _visitors;
        /// <summary>
        /// Интерфейс для логгера.
        /// </summary>
        private ILogger _logger;


        public override string CommandText { get => _innerCommand.CommandText; set => _innerCommand.CommandText = value; }
        public override int CommandTimeout { get => _innerCommand.CommandTimeout; set => _innerCommand.CommandTimeout = value; }
        public override CommandType CommandType { get => _innerCommand.CommandType; set => _innerCommand.CommandType = value; }
        public override UpdateRowSource UpdatedRowSource { get => _innerCommand.UpdatedRowSource; set => _innerCommand.UpdatedRowSource = value; }
        public override bool DesignTimeVisible { get => _innerCommand.DesignTimeVisible; set => _innerCommand.DesignTimeVisible = value; }
        protected override DbConnection? DbConnection { get => _innerCommand.Connection; set => _innerCommand.Connection = value; }
        protected override DbParameterCollection DbParameterCollection => _innerCommand.Parameters;
        protected override DbTransaction? DbTransaction
        {
            get => _innerCommand.Transaction;
            set
            {
                if (value is NDbTransaction ndbtrs)
                {
                    _innerCommand.Transaction = ndbtrs.GetTransaction();
                }
                else
                {
                    _innerCommand.Transaction = value;
                }
            }
        }

        /// <summary>
        /// Конструктор на основе другой команды.
        /// </summary>
        public NDbCommand(DbCommand innerCommand, IEnumerable<IVisitor> visitors, ILogger Logger)
        {
            _executionSteps = new List<ExecutionCommandStep>
            {
                new PrepareCommand(),
                new BeforeExecute(),
                new ExecutionStep(),
                new AfterExecution(),
                new ResultProcessing()
            };
            _visitors = visitors.ToList();
            _logger = Logger;
            foreach (var visitor in _visitors)
            {
                visitor.Logger = Logger;
            }
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            _innerCommand = innerCommand;
        }

        /// <summary>
        /// Конструктор для команды в транзакции.
        /// Привязывает команду к определённому соединению и транзакции.
        /// </summary>
        public NDbCommand(DbCommand innerCommand, DbConnection connection, IEnumerable<IVisitor> visitors, ILogger Logger, DbTransaction? transaction = null)
        {
            _executionSteps = new List<ExecutionCommandStep>
            {
                new PrepareCommand(),
                new BeforeExecute(),
                new ExecutionStep(),
                new AfterExecution(),
                new ResultProcessing()
            };
            _visitors = visitors.ToList();
            _logger = Logger;
            foreach (var visitor in _visitors)
            {
                visitor.Logger = Logger;
            }
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            _innerCommand = innerCommand;
            if (connection is NDbConnection con && transaction is NDbTransaction trs)
            {
                _innerCommand.Connection = con.GetConnection();
                _innerCommand.Transaction = trs.GetTransaction();
            }
        }

        public override void Cancel()
        {
            _innerCommand.Cancel();
        }

        public new IDbDataParameter CreateParameter()
        {
            return _innerCommand.CreateParameter();
        }

        public void Dispose()
        {
            _innerCommand.Dispose();
        }

        /// <summary>
        /// Данный метод вызывает всех посетителей на каждом шагу и обрабатывает ошибки.
        /// Выполнение шагов для асинхронных методов.
        /// </summary>
        private async Task<T> ExecuteStepsAsync<T>(Func<Task<T>?> executeFunc, ExecutionContext executionContext)
        {
            executionContext.ExecutionFunction = executeFunc;
            foreach(var visitor in _visitors)
            {
                executionContext.HadError[visitor] = false;
            }

            foreach (var step in _executionSteps)
            {
                foreach (var visitor in _visitors)
                {
                    /// Если посетитель уже ошибался на более ранних шагах, то его выполнение пропускается.
                    if (executionContext.HadError[visitor])
                    {
                        continue;
                    }
                    try
                    {
                        await step.AcceptVisitorAsync(visitor, executionContext);
                    }
                    catch (Exception ex)
                    {
                        executionContext.HadError[visitor] = true;
                        _logger.LogInfo(ex.Message);
                    }
                }
            }
            if (executionContext.Result == null)
            {
                throw new Exception("Command execution failed, result is null");
            }
            return (T)executionContext.Result;
        }

        /// <summary>
        /// Данный метод вызывает всех посетителей на каждом шагу и обрабатывает ошибки.
        /// Выполнение шагов для синхронных методов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executeFunc"></param>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private T ExecuteSteps<T>(Func<object?> executeFunc, ExecutionContext executionContext)
        {
            executionContext.ExecutionFunction = executeFunc;
            foreach (var visitor in _visitors)
            {
                executionContext.HadError[visitor] = false;
            }

            foreach (var step in _executionSteps)
            {
                foreach (var visitor in _visitors)
                {
                    if (executionContext.HadError[visitor])
                    {
                        continue;
                    }
                    try
                    {
                        step.AcceptVisitor(visitor, executionContext);
                    }
                    catch (Exception ex)
                    {
                        executionContext.HadError[visitor] = true;
                        _logger.LogInfo(ex.Message);
                    }
                }
            }
            if (executionContext.Result == null)
            {
                throw new Exception("Command execution failed, result is null");
            }
            return (T)executionContext.Result;
        }

        /// <summary>
        /// Переопределённый метод выполнения запросов.
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<int>(() => _innerCommand.ExecuteNonQuery(), executionContext);
            _logger.LogInfo("Successful execution");
            executionContext.Result = result;
            return result;
        }

        /// <summary>
        /// Асинхронная версия метода для выполнения запросов.
        /// Для выполнения запросов по типу insert, update, delete
        /// </summary>
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken token)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = await ExecuteStepsAsync(() => _innerCommand.ExecuteNonQueryAsync(), executionContext);
            _logger.LogInfo("Successful async execution");
            executionContext.Result = result;
            return result;
        }


        /// <summary>
        /// Для выполнения запросов, возвращающих одно конкретное значение.
        /// </summary>
        public override object? ExecuteScalar()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<object?>(() => _innerCommand.ExecuteScalar(), executionContext);
            _logger.LogInfo("Successful execution");
            executionContext.Result = result;
            return result;
        }

        /// <summary>
        /// Для асинхронного выполнения запросов, возвращающих одно конкретное значение.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public override async Task<object?> ExecuteScalarAsync(CancellationToken token)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;
            var result = await ExecuteStepsAsync(() => _innerCommand.ExecuteScalarAsync(), executionContext);
            _logger.LogInfo("Successful async execution");
            executionContext.Result = result;
            return result;
        }

        public override void Prepare()
        {
            _innerCommand.Prepare();
        }


        protected override DbParameter CreateDbParameter()
        {
            return _innerCommand.CreateParameter();
        }

        /// <summary>
        /// Переопределённый метод запуска считывателя строк таблицы. 
        /// Тоже проходит через всех посетителей.
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<DbDataReader>(() => _innerCommand.ExecuteReader(behavior), executionContext);
            _logger.LogInfo("Successful execution");
            executionContext.Result = result;
            return result;
        }
    }
}
