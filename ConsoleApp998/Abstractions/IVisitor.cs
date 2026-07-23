using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbGovernator.Realisations;

namespace DbGovernator.Abstractions
{
    /// <summary>
    /// Интерфейс для механизмов, работающих по ходу различных этапов выполнения команды.
    /// </summary>
    public interface IVisitor
    {
        public bool HadException { get; set; }
        public ILogger Logger { get; set; }

        public void VisitPreparing(PrepareCommand step);
        public async Task VisitPreparingAsync(PrepareCommand step) { }
        public void VisitBeforeExecution(BeforeExecute step);
        public async Task VisitBeforeExecutionAsync(BeforeExecute step) { }
        public void VisitExecution(ExecutionSt step);
        public async Task VisitExecutionAsync(ExecutionSt step) { }
        public void VisitAfterExecution(AfterExecution step);
        public async Task VisitAfterExecutionAsync(AfterExecution step) { }
        public void VisitResultProcessing(ResultProcessing step);
        public async Task VisitResultProcessingAsync(ResultProcessing step) { }
    }
}
