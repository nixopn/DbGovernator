using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    /// <summary>
    /// Шаг начала транзакции. Вызывается в конструкторе транзакции.
    /// </summary>
    public class BeginTransactionStep : ExecutionTransactionSteps
    {
        public override void AcceptVisitor(ITransactionVisitor visitor, TransactionContext context)
        {
            visitor.VisitBegin(this);
        }

        public override async Task AcceptVisitorAsync(ITransactionVisitor visitor, TransactionContext context)
        {
            await visitor.VisitBeginAsync(this);
        }
    }

    /// <summary>
    /// Шаг сохранения изменений транзакции.
    /// </summary>
    public class CommitTransactionStep : ExecutionTransactionSteps
    {
        public override void AcceptVisitor(ITransactionVisitor visitor, TransactionContext context)
        {
            visitor.VisitCommit(this);
        }

        public override async Task AcceptVisitorAsync(ITransactionVisitor visitor, TransactionContext context)
        {
            await visitor.VisitCommitAsync(this);
        }
    }

    /// <summary>
    /// Шаг отмены изменений транзакции.
    /// </summary>
    public class RollbackTransactionStep : ExecutionTransactionSteps
    {
        public override void AcceptVisitor(ITransactionVisitor visitor, TransactionContext context)
        {
            visitor.VisitRollback(this);
        }

        public override async Task AcceptVisitorAsync(ITransactionVisitor visitor, TransactionContext context)
        {
            await visitor.VisitRollbackAsync(this);
        }
    }


}
