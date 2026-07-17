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
            visitor.VisitBeforeExecution(context);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            await visitor.VisitBeforeExecutionAsync(context);
        }
    }

    public class PrepareCommand : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitPreparing(context);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            await visitor.VisitPreparingAsync(context);
        }
    }

    public class AfterExecution : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitAfterExecution(context);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            await visitor.VisitAfterExecutionAsync(context);
        }
    }

    public class ResultProcessing : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitResultProcessing(context);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            await visitor.VisitResultProcessingAsync(context);
        }
    }

    public class ExecutionSt : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitExecution(context);
        }

        public async override Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context)
        {
            await visitor.VisitExecutionAsync(context);
        }
    }
}
