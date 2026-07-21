using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    // Собирает следующие метрики:
    // Время запроса
    // Количество затронутых столбцов
    // Сырой sql-запрос
    public class Metrics : IVisitor
    {
        public bool hadException { get; set; }
        public ILogger Logger { get; set; }

        public void VisitResultProcessing(ResultProcessing step)
        {
            Logger.Log($"Duration: {step.Context.Duration.TotalMilliseconds}");
            Logger.Log($"Rows Affected: {step.Context.affectedRows}");
        }

        public async Task VisitResultProcessingAsync(ResultProcessing step)
        {
            Logger.Log($"Duration: {step.Context.Duration.TotalMilliseconds}");
            Logger.Log($"Rows Affected: {step.Context.affectedRows}");
        }

        public void VisitAfterExecution(AfterExecution step)
        {
            step.Context.After = DateTime.UtcNow;
        }

        public async Task VisitAfterExecutionAsync(AfterExecution step)
        { 
            step.Context.After = DateTime.UtcNow; 
        }

        public void VisitBeforeExecution(BeforeExecute step)
        {
            step.Context.Before = DateTime.UtcNow;
        }

        public async Task VisitBeforeExecutionAsync(BeforeExecute step)
        { 
            step.Context.Before = DateTime.UtcNow; 
        }

        public void VisitExecution(ExecutionSt step) { }
        public async Task VisitExecutionAsync(ExecutionSt step) { }

        public void VisitPreparing(PrepareCommand step)
        {
            Logger.Log($"SQL: {step.Context.Command.CommandText}");
        }

        public async Task VisitPreparingAsync(PrepareCommand step)
        { 
            Logger.Log($"SQL: {step.Context.Command.CommandText}");
        }
    }
}
