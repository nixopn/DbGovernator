using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    public class TransactionMetrics : ITransactionVisitor
    {
        public bool hadException {  get; set; }
        public ILogger Logger { get; set; }

        public void VisitBegin(BeginTransactionStep step)
        {
            step.Context.Before = DateTime.UtcNow;
        }

        public void VisitCommit(CommitTransactionStep step)
        {
            step.Context.After = DateTime.UtcNow;
            step.Context.Success = true;
            Logger.Log($"Duration {step.Context.Duration.TotalMilliseconds}");
            Logger.Log($"Success {step.Context.Success}");
        }

        public void VisitRollback(RollbackTransactionStep step)
        {
            step.Context.After = DateTime.UtcNow;
            step.Context.Success = false;
            Logger.Log($"Duration {step.Context.Duration.TotalMilliseconds}");
            Logger.Log($"Success {step.Context.Success}");
        }
    }
}
