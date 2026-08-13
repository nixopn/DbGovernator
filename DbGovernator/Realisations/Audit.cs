using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    /// <summary>
    /// Класс-посетитель команды, собирающий информацию о типе запроса и названии задействованной таблицы.
    /// </summary>
    public class Audit : IVisitor
    {
        public ILogger Logger { get; set; }


        // Собирает информацию о типе запроса и названии таблицы, на которую он действует
        public void VisitResultProcessing(ResultProcessing step) { }

        public void VisitAfterExecution(AfterExecution step) { }

        public void VisitBeforeExecution(BeforeExecute step) { }

        public void VisitExecution(ExecutionStep step) { }
        
        
        public async Task VisitBeforeExecutionAsync(BeforeExecute step) { }
        
        public async Task VisitExecutionAsync(ExecutionStep step) { }
        
        public async Task VisitAfterExecutionAsync(AfterExecution step) { }
        
        public async Task VisitResultProcessingAsync(ResultProcessing step) { }

        /// <summary>
        /// Собирает информацию о типе запроса и названии таблицы, на которую он действует.
        /// </summary>
        /// <param name="step"></param>
        public void VisitPreparing(PrepareCommand step)
        {
            var commandText = step.Context.Command.CommandText;
            if (commandText.ToLower().Contains("select"))
            {
                step.Context.QueryType = "select";
            }
            else if (commandText.ToLower().Contains("update"))
            {
                step.Context.QueryType = "update";
            }
            else if (commandText.ToLower().Contains("insert"))
            {
                step.Context.QueryType = "insert";
            }
            else if (commandText.ToLower().Contains("delete"))
            {
                step.Context.QueryType = "delete";
            }
            else
            {
                step.Context.QueryType = "unkown";
            }
            var patterns = new[]
            {
                @"(?i)(from|into|update|delete\s+from)\s+(\w+)"
            };
            var match = Regex.Match(commandText, patterns[0]);
            step.Context.TableName = match.Groups[2].Value;


            Logger.LogInfo($"Query type: {step.Context.QueryType}");
            Logger.LogInfo($"Table name: {step.Context.TableName}");
        }

        /// <summary>
        /// Асинхронно собирает информацию о типе запроса и названии таблицы, на которую он действует.
        /// </summary>
        /// <param name="step"></param>
        public async Task VisitPreparingAsync(PrepareCommand step)
        {
            var commandText = step.Context.Command.CommandText;
            if (commandText.ToLower().Contains("select"))
            {
                step.Context.QueryType = "select";
            }
            else if (commandText.ToLower().Contains("update"))
            {
                step.Context.QueryType = "update";
            }
            else if (commandText.ToLower().Contains("insert"))
            {
                step.Context.QueryType = "insert";
            }
            else if (commandText.ToLower().Contains("delete"))
            {
                step.Context.QueryType = "delete";
            }
            else
            {
                step.Context.QueryType = "unkown";
            }
            var patterns = new[]
            {
                @"(?i)(from|into|update|delete\s+from)\s+(\w+)"
            };
            var match = Regex.Match(commandText, patterns[0]);
            step.Context.TableName = match.Groups[2].Value;


            Logger.LogInfo($"Query type: {step.Context.QueryType}");
            Logger.LogInfo($"Table name: {step.Context.TableName}");
        }
    }

}
