using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Реализует интерфейс IDbConnection
    internal class NDbConnection : IDbConnection
    {
        private DbConnection _innerConnection;
        public string ConnectionString { get => _innerConnection.ConnectionString; set => _innerConnection.ConnectionString = value; }

        public int ConnectionTimeout => _innerConnection.ConnectionTimeout;

        public string Database => _innerConnection.Database;

        public ConnectionState State => _innerConnection.State;
        public NDbConnection(DbConnection innerConnection)
        {
            _innerConnection = innerConnection;
        }

        public IDbTransaction BeginTransaction()
        {
            return _innerConnection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _innerConnection.BeginTransaction(il);
        }

        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            var ret = await _innerConnection.BeginTransactionAsync();
            return ret;
        }

        public async Task<IDbTransaction> BeginTransactionAsync(IsolationLevel il)
        {
            var ret = await _innerConnection.BeginTransactionAsync(il);
            return ret;
        }


        public void ChangeDatabase(string databaseName)
        {
            _innerConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            _innerConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _innerConnection.CreateCommand();
        }

        public void Dispose()
        {
            _innerConnection.Dispose();
        }

        public void Open()
        {
            _innerConnection.Open();
        }
    }
}
