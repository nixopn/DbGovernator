using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Интерфейс посетителей
    // Имеет методы на все шаги выполнения команд
    interface IVisitor
    {
        public bool hadException { get; set; }
        public ILogger Logger { get; set; }

        public void VisitPreparing(ExecutionContext context);
        public void VisitBeforeExecution(ExecutionContext context);
        public void VisitExecution(ExecutionContext context);
        public void VisitAfterExecution(ExecutionContext context);
        public void VisitResultProcessing(ExecutionContext context);
    }
}
