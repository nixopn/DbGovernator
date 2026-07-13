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







        public async Task<int> ExecuteNonQueryAsync()
        {
            var executionContext = new ExecutionContext();
            executionContext.Command = _innerCommand;
            _executionSteps[1].AcceptVisitor(_visitors[1], executionContext);
            var ret = _innerCommand.ExecuteNonQueryAsync();
            _executionSteps[3].AcceptVisitor(_visitors[1], executionContext);
            executionContext.affectedRows = ret.Result;
            _executionSteps[4].AcceptVisitor(_visitors[1], executionContext);
            return ret.Result;
        }


        public IDataReader ExecuteReader()
        {
            return _innerCommand.ExecuteReader();
        }

        public async Task<DbDataReader> ExecuteReaderAsync()
        {
            var ret = _innerCommand.ExecuteReaderAsync();
            return ret.Result;
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return _innerCommand.ExecuteReader(behavior);
        }

        public async Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            var ret = _innerCommand.ExecuteReaderAsync(behavior);
            return ret.Result;
        }
        public async Task DisposeAsync()
        {
            _innerCommand.DisposeAsync();
        }

        public object? ExecuteScalar()
        {
            return _innerCommand.ExecuteScalar();
        }

        public void Prepare()
        {
            _innerCommand.Prepare();
        }
    }
}
