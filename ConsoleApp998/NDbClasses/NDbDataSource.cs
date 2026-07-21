using DbGovernator.Abstractions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.NDbClasses
{
    // Реализует DbDataSource
    public class NDbDataSource : DbDataSource
    {
        private DbDataSource _innerDataSource;
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;

        public override string ConnectionString => _innerDataSource.ConnectionString;
        public NDbDataSource(DbDataSource innerDataSource, IEnumerable<IVisitor> visitors,  ILogger logger, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            _innerDataSource = innerDataSource;
            _visitors = visitors;
            _logger = logger;
            _transactionVisitors = transactionVisitors;
        }

        protected override DbConnection CreateDbConnection()
        {
            return new NDbConnection(_innerDataSource.CreateConnection(), _visitors, _logger, _transactionVisitors);
        }


        protected override DbCommand CreateDbCommand(string? commandText = null)
        {
            var innerCommand = _innerDataSource.CreateCommand(commandText);
            return new NDbCommand(innerCommand, _visitors, _logger);
        }


        public async Task<DbConnection> OpenConnectionAsync()
        {
            var connection = await _innerDataSource.OpenConnectionAsync();
            return new NDbConnection(connection, _visitors, _logger, _transactionVisitors);
        }

        public DbConnection OpenConnection()
        {
            var returncon = new NDbConnection(_innerDataSource.OpenConnection(), _visitors, _logger, _transactionVisitors);
            return returncon;
        }
    }

}

