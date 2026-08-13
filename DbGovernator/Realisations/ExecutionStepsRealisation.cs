using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    /// <summary>
    /// Класс, реализующий абстрактный класс ExecutionStep шага выполнения команды.
    /// Шаг до выполнения команды.
    /// </summary>
    public class BeforeExecute : ExecutionCommandStep
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
    /// <summary>
    /// Класс, реализующий абстрактный класс ExecutionStep шага выполнения команды.
    /// Шаг подготовки команды.
    /// </summary>
    public class PrepareCommand : ExecutionCommandStep
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

    /// <summary>
    /// Класс, реализующий абстрактный класс ExecutionStep шага выполнения команды.
    /// Шаг после выполнения команды.
    /// </summary>
    public class AfterExecution : ExecutionCommandStep
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

    /// <summary>
    /// Класс, реализующий абстрактный класс ExecutionStep шага выполнения команды.
    /// Шаг обработки результата.
    /// </summary>
    public class ResultProcessing : ExecutionCommandStep
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

    /// <summary>
    /// Класс, реализующий абстрактный класс ExecutionStep шага выполнения команды.
    /// Шаг выполнения команды.
    /// </summary>
    public class ExecutionStep : ExecutionCommandStep
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
