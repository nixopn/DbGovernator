using DbGovernator.Abstractions;
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

        public override string ConnectionString => _innerDataSource.ConnectionString;
        public NDbDataSource(DbDataSource innerDataSource, IEnumerable<IVisitor> visitors, ILogger logger)
        {
            _innerDataSource = innerDataSource;
            _visitors = visitors;
            _logger = logger;
        }

        protected override DbConnection CreateDbConnection()
        {
            return new NDbConnection(_innerDataSource.CreateConnection(), _visitors, _logger);
        }
        public new NDbCommand CreateCommand(string? commandText = null)
        {
            var cmd = base.CreateCommand(commandText);
            var retCmd = new NDbCommand(cmd, _visitors, _logger);
            return retCmd;
        }


        public async Task<DbConnection> OpenConnectionAsync()
        {
            var connection = await _innerDataSource.OpenConnectionAsync();
            return connection;
        }

        public DbConnection OpenConnection()
        {
            var returncon = new NDbConnection(_innerDataSource.OpenConnection(), _visitors, _logger);
            return returncon;
        }
    }
}

