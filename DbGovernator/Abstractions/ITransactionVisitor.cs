using DbGovernator.Realisations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    /// <summary>
    /// Интерфейс для механизмов, работающих по ходу различных этапов транзакции.
    /// </summary>
    public interface ITransactionVisitor
    {
        public ILogger Logger { get; set; }


        public void VisitBegin(BeginTransactionStep step);
        public async Task VisitBeginAsync(BeginTransactionStep step) { }
        public void VisitCommit(CommitTransactionStep step);
        public async Task VisitCommitAsync(CommitTransactionStep step) { }
        public void VisitRollback(RollbackTransactionStep step);
        public async Task VisitRollbackAsync(RollbackTransactionStep step) { }
        
    }
}
