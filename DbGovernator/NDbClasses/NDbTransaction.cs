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
    /// <summary>
    /// Реализует абстрактный класс DbTransaction.
    /// Является обёрткой для всех других реализаций DbTransaction.
    /// </summary>
    public class NDbTransaction : DbTransaction
    {
        private DbTransaction _innerTransaction;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        private ILogger _logger;
        private TransactionContext _transactionContext;

        public override IsolationLevel IsolationLevel => _innerTransaction.IsolationLevel;

        protected override DbConnection? DbConnection => _innerTransaction?.Connection;

        /// <summary>
        /// Конструктор транзакции.
        /// </summary>
        /// <param name="innerTransaction">Оборачиваемая транзакция.</param>
        /// <param name="visitors">Посетители транзакции. Получаются от соединения.</param>
        /// <param name="logger">Логгер. Получается от соединения.</param>
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

        /// <summary>
        /// Сохраняет изменения транзакции.
        /// Принимает посетителей.
        /// </summary>
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

        /// <summary>
        /// Отменяет изменения транзакции.
        /// Принимает посетителей.
        /// </summary>
        public override void Rollback()
        {
            RollbackTransactionStep step = new RollbackTransactionStep();
            foreach (var visitor in _transactionVisitors)
            {
                visitor.VisitRollback(step);
            }
            _innerTransaction?.Rollback();
        }

        /// <summary>
        /// Асинхронно сохраняет изменения транзакции.
        /// Принимает посетителей.
        /// </summary>
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

        /// <summary>
        /// Асинхронно отменяет изменения транзакции.
        /// Принимает посетителей.
        /// </summary>
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
