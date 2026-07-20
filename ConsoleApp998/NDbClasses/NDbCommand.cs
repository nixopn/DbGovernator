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
            var executionStrategy = _visitors.OfType<ExecutionStrategy>().FirstOrDefault();
            if (executionStrategy != null)
            {
                executionStrategy.SetTries(9);
                executionStrategy.SetRetryDelay(298);
            }
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            _innerCommand = innerCommand;
        }



        // Конструктор для команды в транзакции
        public NDbCommand(DbCommand innerCommand, DbConnection connection, DbTransaction transaction, IEnumerable<IVisitor> visitors, ILogger Logger)
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
            var executionStrategy = _visitors.OfType<ExecutionStrategy>().FirstOrDefault();
            if (executionStrategy != null)
            {
                executionStrategy.SetTries(9);
                executionStrategy.SetRetryDelay(298);
            }
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            _innerCommand = innerCommand;
            if (connection is NDbConnection con && transaction is NDbTransaction trs)
            {
                _innerCommand.Connection = con.GetConnection();
                _innerCommand.Transaction = trs.GetTransaction();
            }
            //_innerCommand.Connection = connection;
            //_innerCommand.Transaction = transaction;
            //Connection = connection;
            //Transaction = transaction;
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
            //var executionContext = new ExecutionContext();
            //executionContext.Command = _innerCommand;
            //_executionSteps[1].AcceptVisitor(_visitors[1], executionContext);
            //var ret = _innerCommand.ExecuteNonQueryAsync();
            //_executionSteps[3].AcceptVisitor(_visitors[1], executionContext);
            //executionContext.affectedRows = ret.Result;
            //_executionSteps[4].AcceptVisitor(_visitors[1], executionContext);
            //return ret.Result;
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = await ExecuteStepsAsync(() => _innerCommand.ExecuteNonQueryAsync(), executionContext);
            executionContext.Result = result;
            return result;
        }


        // Для выполнения запросов по типу select
        public new IDataReader ExecuteReader()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<IDataReader>(() => _innerCommand.ExecuteReader(), executionContext);
            executionContext.Result = result;
            return result;
            // return _innerCommand.ExecuteReader();
        }

        public new async Task<DbDataReader> ExecuteReaderAsync()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = await ExecuteStepsAsync(() => _innerCommand.ExecuteReaderAsync(), executionContext);
            executionContext.Result = result;
            return result;
            //var ret = _innerCommand.ExecuteReaderAsync();
            //return ret.Result;
        }

        public new IDataReader ExecuteReader(CommandBehavior behavior)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<IDataReader>(() => _innerCommand.ExecuteReader(behavior), executionContext);
            executionContext.Result = result;
            return result;
            //return _innerCommand.ExecuteReader(behavior);
        }

        public new async Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = await ExecuteStepsAsync(() => _innerCommand.ExecuteReaderAsync(behavior), executionContext);
            executionContext.Result = result;
            return result;
            //var ret = _innerCommand.ExecuteReaderAsync(behavior);
            //return ret.Result;
        }
        public new async Task DisposeAsync()
        {
            await _innerCommand.DisposeAsync();
        }


        // Для выполнения запросов, возвращающих одно конкретное значение
        public override object? ExecuteScalar()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<object?>(() => _innerCommand.ExecuteScalar(), executionContext);
            executionContext.Result = result;
            return result;
            //return _innerCommand.ExecuteScalar();
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
            //return _innerCommand.ExecuteReader(behavior);
        }
    }
}
