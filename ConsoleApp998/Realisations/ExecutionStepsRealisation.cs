using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    public class BeforeExecute : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            visitor.VisitBeforeExecution(this);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            await visitor.VisitBeforeExecutionAsync(this);
        }
    }

    public class PrepareCommand : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            visitor.VisitPreparing(this);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            await visitor.VisitPreparingAsync(this);
        }
    }

    public class AfterExecution : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            visitor.VisitAfterExecution(this);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            await visitor.VisitAfterExecutionAsync(this);
        }
    }

    public class ResultProcessing : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            visitor.VisitResultProcessing(this);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            await visitor.VisitResultProcessingAsync(this);
        }
    }

    public class ExecutionSt : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            visitor.VisitExecution(this);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            Context = context;
            await visitor.VisitExecutionAsync(this);
        }
    }
}
