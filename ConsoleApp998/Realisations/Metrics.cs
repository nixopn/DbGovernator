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

        public void VisitResultProcessing(ExecutionContext context)
        {
            Logger.Log($"Duration: {context.Duration.TotalMilliseconds}");
            Logger.Log($"Rows Affected: {context.affectedRows}");
        }

        public void VisitAfterExecution(ExecutionContext context)
        {
            context.After = DateTime.Now;
        }

        public void VisitBeforeExecution(ExecutionContext context)
        {
            context.Before = DateTime.Now;
        }

        public void VisitExecution(ExecutionContext context) { }

        public void VisitPreparing(ExecutionContext context)
        {
            Logger.Log($"SQL: {context.Command.CommandText}");
        }
    }
}
