using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp998
{
    internal class NDbCommand : IDbCommand
    {
        private DbCommand _innerCommand;


        private List<ExecutionStep> _executionSteps;
        private List<IVisitor> _visitors;


        public string CommandText { get => _innerCommand.CommandText; set => _innerCommand.CommandText = value; }
        public int CommandTimeout { get => _innerCommand.CommandTimeout; set => _innerCommand.CommandTimeout = value; }
        public CommandType CommandType { get => _innerCommand.CommandType; set => _innerCommand.CommandType = value; }
        public IDbConnection? Connection { get => _innerCommand.Connection; set => _innerCommand.Connection = (System.Data.Common.DbConnection)value; }

        public IDataParameterCollection Parameters => _innerCommand.Parameters;

        public IDbTransaction? Transaction { get => _innerCommand.Transaction; set => _innerCommand.Transaction = (System.Data.Common.DbTransaction)value; }
        public UpdateRowSource UpdatedRowSource { get => _innerCommand.UpdatedRowSource; set => _innerCommand.UpdatedRowSource = value; }
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
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            _executionSteps[0].AcceptVisitor(_visitors[0], executionContext);
            _innerCommand = innerCommand;
        }




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
            var executionContext = new ExecutionContext();
            executionContext.Command = innerCommand;
            _executionSteps[0].AcceptVisitor(_visitors[0], executionContext);
            _innerCommand = innerCommand;
            _innerCommand.Connection = connection;
            _innerCommand.Transaction = transaction;
            Connection = connection;
            Transaction = transaction;
        }


        public void AddVisitor(IVisitor visitor)
        {
            _visitors.Add(visitor);
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
            int ret = _innerCommand.ExecuteNonQuery();
            return ret;
        }



        private async Task<T> ExecuteStepsAsync<T>(Func<Task<T>> executeFunc, ExecutionContext executionContext)
        {
            executionContext.executionFunction = executeFunc;
            foreach (var step in _executionSteps)
            {
                foreach(var visitor in _visitors)
                {
                    step.AcceptVisitor(visitor, executionContext);
                }
            }
            return (T)executionContext.Result;
        }






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


        public IDataReader ExecuteReader()
        {
            return _innerCommand.ExecuteReader();
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
            return _innerCommand.ExecuteReader(behavior);
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

        public object? ExecuteScalar()
        {
            return _innerCommand.ExecuteScalar();
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
