using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Реализует интерфейс IDbCommand, является обёрткой для DbCommand
    internal class NDbCommand : IDbCommand
    {
        private DbCommand _innerCommand;


        // Списки шагов выполнения и посетителей, задаются в конструкторе (пока что)
        private List<ExecutionStep> _executionSteps;
        private List<IVisitor> _visitors;


        // Текст команды
        public string CommandText { get => _innerCommand.CommandText; set => _innerCommand.CommandText = value; }
        public int CommandTimeout { get => _innerCommand.CommandTimeout; set => _innerCommand.CommandTimeout = value; }
        public CommandType CommandType { get => _innerCommand.CommandType; set => _innerCommand.CommandType = value; }
        // Соединение команды
        public IDbConnection? Connection { get => _innerCommand.Connection; set => _innerCommand.Connection = (System.Data.Common.DbConnection)value; }
        // Параметры команды
        public IDataParameterCollection Parameters => _innerCommand.Parameters;
        // Транзакция, частью которой является команда (команда может быть и без транзакции)
        public IDbTransaction? Transaction { get => _innerCommand.Transaction; set => _innerCommand.Transaction = (System.Data.Common.DbTransaction)value; }
        public UpdateRowSource UpdatedRowSource { get => _innerCommand.UpdatedRowSource; set => _innerCommand.UpdatedRowSource = value; }
        // Конструктор на основе другой команды
        public NDbCommand(DbCommand innerCommand)
        {
            _executionSteps = new List<ExecutionStep>
            {
                new PrepareCommand(),
                new BeforeExecute(),
                new ExecutionSt(),
                new AfterExecution(),
                new ResultProcessing()
            };
            _visitors = new List<IVisitor>();
            Audit audit = new Audit();
            AddVisitor(audit);
            Metrics metrics = new Metrics();
            AddVisitor(metrics);
            ExecutionStrategy executionStrategy = new ExecutionStrategy();
            executionStrategy.SetTries(9);
            executionStrategy.SetRetryDelay(298);
            AddVisitor(executionStrategy);
            foreach(var visitor in _visitors)
            {
                visitor.Logger = new ConsoleLogger();
            }
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            _innerCommand = innerCommand;
        }



        // Конструктор для команды в транзакции
        public NDbCommand(DbCommand innerCommand, DbConnection connection, DbTransaction transaction)
        {
            _executionSteps = new List<ExecutionStep>
            {
                new PrepareCommand(),
                new BeforeExecute(),
                new ExecutionSt(),
                new AfterExecution(),
                new ResultProcessing()
            };
            _visitors = new List<IVisitor>();
            Audit audit = new Audit();
            AddVisitor(audit);
            Metrics metrics = new Metrics();
            AddVisitor(metrics);
            ExecutionStrategy executionStrategy = new ExecutionStrategy();
            executionStrategy.SetTries(9);
            executionStrategy.SetRetryDelay(298);
            AddVisitor(executionStrategy);
            foreach (var visitor in _visitors)
            {
                visitor.Logger = new ConsoleLogger();
            }
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            //_executionSteps[0].AcceptVisitor(_visitors[0], executionContext);
            _innerCommand = innerCommand;
            _innerCommand.Connection = connection;
            _innerCommand.Transaction = transaction;
            Connection = connection;
            Transaction = transaction;
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
        public void Cancel()
        {
            _innerCommand.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return _innerCommand.CreateParameter();
        }

        public void Dispose()
        {
            _innerCommand.Dispose();
        }


        public int ExecuteNonQuery()
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
                foreach(var visitor in _visitors)
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
                        Console.WriteLine(ex.Message);
                    }
                    //step.AcceptVisitor(visitor, executionContext);
                }
            }
            if(executionContext.Result == null)
            {
                throw (new Exception("Command execution failed, result is null"));
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
                        Console.WriteLine(ex.Message);
                    }
                    //step.AcceptVisitor(visitor, executionContext);
                }
            }
            if (executionContext.Result == null)
            {
                throw (new Exception("Command execution failed, result is null"));
            }
            return (T)executionContext.Result;
        }





        // Для выполнения запросов по типу insert, update, delete
        public async Task<int> ExecuteNonQueryAsync()
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

            var result = await ExecuteStepsAsync<int>(async () => await _innerCommand.ExecuteNonQueryAsync(), executionContext);
            executionContext.Result = result;
            return result;
        }


        // Для выполнения запросов по типу select
        public IDataReader ExecuteReader()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<IDataReader>(() => _innerCommand.ExecuteReader(), executionContext);
            executionContext.Result = result;
            return result;
            // return _innerCommand.ExecuteReader();
        }

        public async Task<DbDataReader> ExecuteReaderAsync()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = await ExecuteStepsAsync<DbDataReader>(async () => await _innerCommand.ExecuteReaderAsync(), executionContext);
            executionContext.Result = result;
            return result;
            //var ret = _innerCommand.ExecuteReaderAsync();
            //return ret.Result;
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<IDataReader>(() => _innerCommand.ExecuteReader(behavior), executionContext);
            executionContext.Result = result;
            return result;
            //return _innerCommand.ExecuteReader(behavior);
        }

        public async Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = await ExecuteStepsAsync<DbDataReader>(async () => await _innerCommand.ExecuteReaderAsync(behavior), executionContext);
            executionContext.Result = result;
            return result;
            //var ret = _innerCommand.ExecuteReaderAsync(behavior);
            //return ret.Result;
        }
        public async Task DisposeAsync()
        {
            await _innerCommand.DisposeAsync();
        }


        // Для выполнения запросов, возвращающих одно конкретное значение
        public object? ExecuteScalar()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;

            var result = ExecuteSteps<object?>(() => _innerCommand.ExecuteScalar(), executionContext);
            executionContext.Result = result;
            return result;
            //return _innerCommand.ExecuteScalar();
        }

        public async Task<object?> ExecuteScalarAsync()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;
            var result = await ExecuteStepsAsync<object?>(async () => await _innerCommand.ExecuteScalarAsync(), executionContext);
            executionContext.Result = result;
            return result;
            //var ret = _innerCommand.ExecuteScalarAsync();
            //return ret.Result ?? null;
        }

        public void Prepare()
        {
            _innerCommand.Prepare();
        }
    }
}
