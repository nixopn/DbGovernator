using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Реализует IDbTransaction
    internal class NDbTransaction : IDbTransaction
    {
        private DbTransaction _innerTransaction;
        public IDbConnection? Connection => _innerTransaction?.Connection;

        public IsolationLevel IsolationLevel => _innerTransaction.IsolationLevel;

        public NDbTransaction(DbTransaction innerTransaction)
        {
            _innerTransaction = innerTransaction;
        }

        public void Commit()
        {
            _innerTransaction.Commit();
        }

        public void Dispose()
        {
            _innerTransaction?.Dispose();
        }

        public void Rollback()
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
