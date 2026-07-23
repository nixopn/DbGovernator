using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    /// <summary>
    /// Класс-посетитель, собирающий метрики с транзакций.
    /// </summary>
    public class TransactionMetrics : ITransactionVisitor
    {
        public ILogger Logger { get; set; }

        /// <summary>
        /// Собирает время перед транзакцией.
        /// </summary>
        /// <param name="step"></param>
        public void VisitBegin(BeginTransactionStep step)
        {
            step.Context.Before = DateTime.UtcNow;
        }
        /// <summary>
        /// Собирает время после транзакции, сохраняет её результат(Успешная или нет) и логирует их.
        /// </summary>
        /// <param name="step"></param>
        public void VisitCommit(CommitTransactionStep step)
        {
            step.Context.After = DateTime.UtcNow;
            step.Context.Success = true;
            Logger.LogInfo($"Duration {step.Context.Duration.TotalMilliseconds}");
            Logger.LogInfo($"Success {step.Context.Success}");
        }

        /// <summary>
        /// Собирает время после транзакции, сохраняет её результат(Успешная или нет) и логирует их.
        /// </summary>
        /// <param name="step"></param>
        public void VisitRollback(RollbackTransactionStep step)
        {
            step.Context.After = DateTime.UtcNow;
            step.Context.Success = false;
            Logger.LogInfo($"Duration {step.Context.Duration.TotalMilliseconds}");
            Logger.LogInfo($"Success {step.Context.Success}");
        }
    }
}
