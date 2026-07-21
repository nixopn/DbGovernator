using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    public abstract class ExecutionTransactionSteps
    {
        public ExecutionContext Context { get; set; } = new ExecutionContext();
        public abstract void AcceptVisitor(IVisitor visitor, ExecutionContext context);
        public abstract Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context);
    }
}
