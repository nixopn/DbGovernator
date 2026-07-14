using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;





namespace DbGovernator
{

    // Абстрактный класс, определяющий шаги выполнения команд.
    // Все конкретные классы являются этапами работы команды и принимают следующие параметры.
    // Посетителя, выполняющего свои функции на этом этапе.
    // Контекст исполнения, принимается посетителем.
    internal abstract class ExecutionStep
    {
        public abstract void AcceptVisitor(IVisitor visitor, ExecutionContext context);
    }


    class BeforeExecute : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitBeforeExecution(context);
        }
    }

    class PrepareCommand : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitPreparing(context);
        }
    }

    class AfterExecution : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitAfterExecution(context);
        }
    }

    class ResultProcessing : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitResultProcessing(context);
        }
    }

    class ExecutionSt : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitExecution(context);
        }
    }
}


