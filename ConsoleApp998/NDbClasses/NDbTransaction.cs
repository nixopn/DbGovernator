using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.NDbClasses
{
    // Реализует IDbTransaction
    public class NDbTransaction : DbTransaction
    {
        private DbTransaction _innerTransaction;
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;

        public override IsolationLevel IsolationLevel => _innerTransaction.IsolationLevel;

        protected override DbConnection? DbConnection => _innerTransaction?.Connection;

        public NDbTransaction(DbTransaction innerTransaction, IEnumerable<IVisitor> visitors, ILogger logger)
        {
            _innerTransaction = innerTransaction;
            _visitors = visitors;
            _logger = logger;
        }

        public override void Commit()
        {
            _innerTransaction.Commit();
        }

        public void Dispose()
        {
            _innerTransaction?.Dispose();
        }

        public override void Rollback()
        {
            _innerTransaction?.Rollback();
        }

        public async Task CommitAsync()
        {
            await _innerTransaction.CommitAsync();
        }

        public async Task DisposeAsync()
        {
            await _innerTransaction.DisposeAsync();
        }

        public async Task RollbackAsync()
        {
            await _innerTransaction.RollbackAsync();
        }
    }
}
