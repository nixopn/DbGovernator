using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    public abstract class ExecutionTransactionSteps
    {
        public TransactionContext Context { get; set; } = new TransactionContext();
        public abstract void AcceptVisitor(ITransactionVisitor visitor, TransactionContext context);
        public abstract Task AcceptVisitorAsync(ITransactionVisitor visitor, TransactionContext context);
    }
}
