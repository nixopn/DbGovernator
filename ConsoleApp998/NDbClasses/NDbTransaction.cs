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
    // Реализует IDbTransaction
    public class NDbTransaction : DbTransaction
    {
        private DbTransaction _innerTransaction;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        private ILogger _logger;
        private TransactionContext _transactionContext;

        public override IsolationLevel IsolationLevel => _innerTransaction.IsolationLevel;

        protected override DbConnection? DbConnection => _innerTransaction?.Connection;

        public NDbTransaction(DbTransaction innerTransaction, IEnumerable<ITransactionVisitor> visitors, ILogger logger)
        {
            BeginTransactionStep step = new BeginTransactionStep();
            foreach (var visitor in visitors)
            {
                visitor.Logger = logger;
                visitor.VisitBegin(step);
            }
            _transactionContext = step.Context;
            _innerTransaction = innerTransaction;
            _transactionVisitors = visitors;
            _logger = logger;
        }

        public override void Commit()
        {
            CommitTransactionStep step = new CommitTransactionStep();
            foreach (var visitor in _transactionVisitors)
            {
                visitor.VisitCommit(step);
            }
            _innerTransaction.Commit();
        }

        public void Dispose()
        {
            _innerTransaction?.Dispose();
        }

        public override void Rollback()
        {
            RollbackTransactionStep step = new RollbackTransactionStep();
            foreach (var visitor in _transactionVisitors)
            {
                visitor.VisitRollback(step);
            }
            _innerTransaction?.Rollback();
        }

        public async Task CommitAsync()
        {
            CommitTransactionStep step = new CommitTransactionStep();
            foreach (var visitor in _transactionVisitors)
            {
                await visitor.VisitCommitAsync(step);
            }
            await _innerTransaction.CommitAsync();
        }

        public async Task DisposeAsync()
        {
            await _innerTransaction.DisposeAsync();
        }

        public async Task RollbackAsync()
        {
            RollbackTransactionStep step = new RollbackTransactionStep();
            foreach (var visitor in _transactionVisitors)
            {
                await visitor.VisitRollbackAsync(step);
            }
            await _innerTransaction.RollbackAsync();
        }


        public DbTransaction GetTransaction()
        {
            return _innerTransaction;
        }
    }
}
