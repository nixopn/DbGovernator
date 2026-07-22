using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
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
