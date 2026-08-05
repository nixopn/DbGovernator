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
            _logger = logger;
            BeginTransactionStep step = new BeginTransactionStep();
            foreach (var visitor in visitors)
            {
                try
                {
                    visitor.Logger = logger;
                    visitor.VisitBegin(step);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception at BeginTransactionStep \n {ex.Message}");
                }
            }
            _innerTransaction = innerTransaction;
            _transactionVisitors = visitors;
            _logger.LogInfo("Begin step executed successfuly");
            _transactionContext = step.Context;
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
                try
                {
                    visitor.VisitCommit(step);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception at CommitStep \n {ex.Message}");
                }
            }
            _innerTransaction.Commit();
            _logger.LogInfo("Commit step executed successfuly");
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
                try
                {
                    visitor.VisitRollback(step);
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Exception at RollbackStep \n {ex}");
                }
            }
            _innerTransaction?.Rollback();
            _logger.LogInfo("Rollback step executed successfuly");
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
                try
                {
                    await visitor.VisitCommitAsync(step);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception at CommitStepAsync \n {ex.Message}");
                }
            }
            await _innerTransaction.CommitAsync();
            _logger.LogInfo("Commit step executed successfuly");
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
                try
                {
                    await visitor.VisitRollbackAsync(step);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception at RollbackStepAsync \n {ex}");
                }
            }
            await _innerTransaction.RollbackAsync();
            _logger.LogInfo("Rollback step executed successfuly");
        }


        public DbTransaction GetTransaction()
        {
            return _innerTransaction;
        }
    }
}
