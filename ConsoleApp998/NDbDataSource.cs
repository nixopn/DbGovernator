using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Реализует DbDataSource
    internal class NDbDataSource : DbDataSource
    {
        private DbDataSource _innerDataSource;
        public override string ConnectionString => _innerDataSource.ConnectionString;
        public NDbDataSource(DbDataSource innerDataSource)
        {
            _innerDataSource = innerDataSource;
        }

        protected override DbConnection CreateDbConnection()
        {
            return _innerDataSource.CreateConnection();
        }
        public new NDbCommand CreateCommand(string? commandText = null)
        {
            var cmd = base.CreateCommand(commandText);
            var retCmd = new NDbCommand(cmd);
            return retCmd;
        }

        public async Task<DbConnection> OpenConnectionAsync()
        {
            var connection = await _innerDataSource.OpenConnectionAsync();
            return connection;
        }

        public DbConnection OpenConnection()
        {
            return _innerDataSource.OpenConnection();
        }
    }
}

