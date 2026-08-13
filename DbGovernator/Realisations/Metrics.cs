using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    ///<summary>
    /// Собирает следующие метрики:
    /// Время запроса
    /// Количество затронутых столбцов
    /// Сырой sql-запрос
    /// </summary>
    public class Metrics : IVisitor
    {
        public ILogger Logger { get; set; }

        /// <summary>
        /// Логирует время выполнения запроса и затронутые строки.
        /// </summary>
        /// <param name="step"></param>
        public void VisitResultProcessing(ResultProcessing step)
        {
            Logger.LogInfo($"Duration: {step.Context.Duration.TotalMilliseconds}");
            Logger.LogInfo($"Rows Affected: {step.Context.AffectedRows}");
        }

        /// <summary>
        /// Асинхронно логирует время выполнения запроса и затронутые строки.
        /// </summary>
        /// <param name="step"></param>
        public async Task VisitResultProcessingAsync(ResultProcessing step)
        {
            Logger.LogInfo($"Duration: {step.Context.Duration.TotalMilliseconds}");
            Logger.LogInfo($"Rows Affected: {step.Context.AffectedRows}");
        }

        /// <summary>
        /// Собирает время после выполнения запроса.
        /// </summary>
        /// <param name="step"></param>
        public void VisitAfterExecution(AfterExecution step)
        {
            step.Context.After = DateTime.UtcNow;
        }

        /// <summary>
        /// Собирает время после выполнения запроса.
        /// </summary>
        /// <param name="step"></param>
        public async Task VisitAfterExecutionAsync(AfterExecution step)
        { 
            step.Context.After = DateTime.UtcNow; 
        }

        /// <summary>
        /// Собирает время до выполнения запроса.
        /// </summary>
        /// <param name="step"></param>
        public void VisitBeforeExecution(BeforeExecute step)
        {
            step.Context.Before = DateTime.UtcNow;
        }

        /// <summary>
        /// Собирает время до выполнения запроса.
        /// </summary>
        /// <param name="step"></param>
        public async Task VisitBeforeExecutionAsync(BeforeExecute step)
        { 
            step.Context.Before = DateTime.UtcNow; 
        }

        public void VisitExecution(ExecutionStep step) { }
        public async Task VisitExecutionAsync(ExecutionStep step) { }

        /// <summary>
        /// Собирает сырой sql-запрос.
        /// </summary>
        /// <param name="step"></param>
        public void VisitPreparing(PrepareCommand step)
        {
            Logger.LogInfo($"SQL: {step.Context.Command.CommandText}");
        }

        /// <summary>
        /// Собирает сырой sql-запрос.
        /// </summary>
        /// <param name="step"></param>
        public async Task VisitPreparingAsync(PrepareCommand step)
        { 
            Logger.LogInfo($"SQL: {step.Context.Command.CommandText}");
        }
    }
}
