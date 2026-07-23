using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    /// <summary>
    /// Класс-посетитель команды, проверяющий доступ к выполнению определённых команд.
    /// </summary>
    public class AccessChecker : IVisitor
    {
        public bool HadException { get; set; }
        public ILogger Logger { get; set; }

        /// <summary>
        /// Если команда недоступна, то заменяет её на ';', тем самым не выполняя команду.
        /// </summary>
        /// <param name="step"></param>
        /// <exception cref="Exception"></exception>
        public void VisitPreparing(PrepareCommand step)
        {
            if (step.Context.queryType == "delete" && step.Context.tableName == "users")
            {
                step.Context.Command.CommandText = ";";
                throw new Exception("Access denied");
            }
        }
        /// <summary>
        /// Если команда недоступна, то заменяет её на ';', тем самым не выполняя команду.
        /// </summary>
        /// <param name="step"></param>
        /// <exception cref="Exception"></exception>
        public async Task VisitPreparingAsync(PrepareCommand step)
        {
            if (step.Context.queryType == "delete" && step.Context.tableName == "users")
            {
                step.Context.Command.CommandText = ";";
                throw new Exception("Access denied");
            }
        }
        public void VisitBeforeExecution(BeforeExecute step) { }
        public async Task VisitBeforeExecutionAsync(BeforeExecute step) { }
        public void VisitExecution(ExecutionSt step) { }
        public async Task VisitExecutionAsync(ExecutionSt step) { }
        public void VisitAfterExecution(AfterExecution step) { }
        public async Task VisitAfterExecutionAsync(AfterExecution step) { }
        public void VisitResultProcessing(ResultProcessing step) { }
        public async Task VisitResultProcessingAsync(ResultProcessing step) { }
    }
}
