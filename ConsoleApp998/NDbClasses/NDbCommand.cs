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
    // Реализует интерфейс IDbCommand, является обёрткой для DbCommand
    public class NDbCommand : DbCommand
    {
        private DbCommand _innerCommand;


        // Списки шагов выполнения и посетителей, задаются в конструкторе (пока что)
        private List<ExecutionStep> _executionSteps;
        private List<IVisitor> _visitors;
        private ILogger _logger;


        // Текст команды
        public override string CommandText { get => _innerCommand.CommandText; set => _innerCommand.CommandText = value; }
        public override int CommandTimeout { get => _innerCommand.CommandTimeout; set => _innerCommand.CommandTimeout = value; }
        public override CommandType CommandType { get => _innerCommand.CommandType; set => _innerCommand.CommandType = value; }

        // Транзакция, частью которой является команда (команда может быть и без транзакции)
        public override UpdateRowSource UpdatedRowSource { get => _innerCommand.UpdatedRowSource; set => _innerCommand.UpdatedRowSource = value; }
        public override bool DesignTimeVisible { get => _innerCommand.DesignTimeVisible; set => _innerCommand.DesignTimeVisible = value; }
        protected override DbConnection? DbConnection { get => _innerCommand.Connection; set => _innerCommand.Connection = value; }

        protected override DbParameterCollection DbParameterCollection => _innerCommand.Parameters;

        protected override DbTransaction? DbTransaction { get => _innerCommand.Transaction; set => _innerCommand.Transaction = value; }

        // Конструктор на основе другой команды
        public NDbCommand(DbCommand innerCommand, IEnumerable<IVisitor> visitors, ILogger Logger)
        {
            _executionSteps = new List<ExecutionStep>
            {
                new PrepareCommand(),
                new BeforeExecute(),
                new ExecutionSt(),
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



        // Конструктор для команды в транзакции
        public NDbCommand(DbCommand innerCommand, DbConnection connection, IEnumerable<IVisitor> visitors, ILogger Logger, DbTransaction? transaction = null)
        {
            _executionSteps = new List<ExecutionStep>
            {
                new PrepareCommand(),
                new BeforeExecute(),
                new ExecutionSt(),
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





        // Добавляет посетителя
        public void AddVisitor(IVisitor visitor)
        {
            _visitors.Add(visitor);
        }
        // Удаляет посетителя
        public void DeleteVisitor(IVisitor visitor)
        {
            _visitors.Remove(visitor);
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


        public override int ExecuteNonQuery()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<int>(() => _innerCommand.ExecuteNonQuery(), executionContext);
            executionContext.Result = result;
            return result;
            //int ret = _innerCommand.ExecuteNonQuery();
            //return ret;
        }


        // Выполнение шагов для асинхронных методов
        private async Task<T> ExecuteStepsAsync<T>(Func<Task<T>> executeFunc, ExecutionContext executionContext)
        {
            executionContext.executionFunction = executeFunc;
            foreach (var step in _executionSteps)
            {
                foreach (var visitor in _visitors)
                {
                    if (visitor.hadException)
                    {
                        continue;
                    }
                    try
                    {
                        await step.AcceptVisitorAsync(visitor, executionContext);
                    }
                    catch (Exception ex)
                    {
                        visitor.hadException = true;
                        _logger.Log(ex.Message);
                    }
                    //step.AcceptVisitor(visitor, executionContext);
                }
            }
            if (executionContext.Result == null)
            {
                foreach (var visitor in _visitors)
                {
                    visitor.hadException = false;
                }
                throw new Exception("Command execution failed, result is null");
            }
            return (T)executionContext.Result;
        }


        private T ExecuteSteps<T>(Func<object?> executeFunc, ExecutionContext executionContext)
        {
            executionContext.executionFunction = executeFunc;
            foreach (var step in _executionSteps)
            {
                foreach (var visitor in _visitors)
                {
                    if (visitor.hadException)
                    {
                        continue;
                    }
                    try
                    {
                        step.AcceptVisitor(visitor, executionContext);
                    }
                    catch (Exception ex)
                    {
                        visitor.hadException = true;
                        _logger.Log(ex.Message);
                    }
                    //step.AcceptVisitor(visitor, executionContext);
                }
            }
            if (executionContext.Result == null)
            {
                foreach (var visitor in _visitors)
                {
                    visitor.hadException = false;
                }
                throw new Exception("Command execution failed, result is null");
            }
            return (T)executionContext.Result;
        }





        // Для выполнения запросов по типу insert, update, delete
        public override async Task<int> ExecuteNonQueryAsync(CancellationToken token)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = await ExecuteStepsAsync(() => _innerCommand.ExecuteNonQueryAsync(), executionContext);
            executionContext.Result = result;
            return result;
        }



        // Для выполнения запросов, возвращающих одно конкретное значение
        public override object? ExecuteScalar()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<object?>(() => _innerCommand.ExecuteScalar(), executionContext);
            executionContext.Result = result;
            return result;
        }

        public override async Task<object?> ExecuteScalarAsync(CancellationToken token)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;
            var result = await ExecuteStepsAsync(() => _innerCommand.ExecuteScalarAsync(), executionContext);
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

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<DbDataReader>(() => _innerCommand.ExecuteReader(behavior), executionContext);
            executionContext.Result = result;
            return result;
        }
    }
}
