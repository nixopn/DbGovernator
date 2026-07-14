using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Собирает следующие метрики:
    // Время запроса
    // Количество затронутых столбцов
    // Сырой sql-запрос
    class Metrics : IVisitor
    {
        public void VisitResultProcessing(ExecutionContext context)
        {
            Console.WriteLine($"Duration: {context.Duration.TotalMilliseconds}");
            Console.WriteLine($"Rows Affected: {context.affectedRows}");
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
            Console.WriteLine($"SQL: {context.Command.CommandText}");
        }
    }
}
