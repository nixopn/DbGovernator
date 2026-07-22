using DbGovernator.Abstractions;
using DbGovernator.Realisations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.NDbClasses
{
    // Реализует интерфейс IDbConnection
    public class NDbConnection : DbConnection
    {
        private DbConnection _innerConnection;
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private DbTransaction _trs;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        public override string ConnectionString { get => _innerConnection.ConnectionString; set => _innerConnection.ConnectionString = value; }

        public override int ConnectionTimeout => _innerConnection.ConnectionTimeout;

        public override string Database => _innerConnection.Database;

        public override ConnectionState State => _innerConnection.State;

        public override string DataSource => _innerConnection.DataSource;

        public override string ServerVersion => _innerConnection.ServerVersion;


        public NDbConnection(DbConnection innerConnection, IEnumerable<IVisitor> visitors, ILogger logger, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            _innerConnection = innerConnection;
            _visitors = visitors;
            _logger = logger;
            _transactionVisitors = transactionVisitors;
        }

        public IDbTransaction BeginTransaction()
        {
            return new NDbTransaction(_innerConnection.BeginTransaction(), _transactionVisitors, _logger);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return new NDbTransaction(_innerConnection.BeginTransaction(il), _transactionVisitors, _logger);
        }

        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            var preret = await _innerConnection.BeginTransactionAsync();
            var ret = new NDbTransaction(preret, _transactionVisitors, _logger);
            return ret;
        }

        public async Task<IDbTransaction> BeginTransactionAsync(IsolationLevel il)
        {
            var preret = await _innerConnection.BeginTransactionAsync(il);
            var ret = new NDbTransaction(preret, _transactionVisitors, _logger);
            return ret;
        }


        public override void ChangeDatabase(string databaseName)
        {
            _innerConnection.ChangeDatabase(databaseName);
        }


        public override void Close()
        {
            _innerConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return new NDbCommand(_innerConnection.CreateCommand(), this,  _visitors, _logger, _trs);
        }

        public void Dispose()
        {
            _innerConnection.Dispose();
        }

        public override void Open()
        {
            _innerConnection.Open();
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            var trs = new NDbTransaction(_innerConnection.BeginTransaction(isolationLevel), _transactionVisitors, _logger);
            _trs = trs;
            return trs;
        }


        protected override DbCommand CreateDbCommand()
        {
            return new NDbCommand(_innerConnection.CreateCommand(), _visitors, _logger);
        }


        public DbConnection GetConnection()
        {
            return _innerConnection;
        }
    }
}
