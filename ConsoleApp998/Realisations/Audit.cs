using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    public class Audit : IVisitor
    {
        public bool hadException { get; set; }
        public ILogger Logger { get; set; }


        // Собирает информацию о типе запроса и названии таблицы, на которую он действует
        public void VisitResultProcessing(ExecutionContext context) { }

        public void VisitAfterExecution(ExecutionContext context) { }

        public void VisitBeforeExecution(ExecutionContext context) { }

        public void VisitExecution(ExecutionContext context) { }
        
        
        public async Task VisitBeforeExecutionAsync(ExecutionContext context) { }
        
        public async Task VisitExecutionAsync(ExecutionContext context) { }
        
        public async Task VisitAfterExecutionAsync(ExecutionContext context) { }
        
        public async Task VisitResultProcessingAsync(ExecutionContext context) { }

        public void VisitPreparing(ExecutionContext context)
        {
            var commandText = context.Command.CommandText;
            if (commandText.ToLower().Contains("select"))
            {
                context.queryType = "select";
            }
            else if (commandText.ToLower().Contains("update"))
            {
                context.queryType = "update";
            }
            else if (commandText.ToLower().Contains("insert"))
            {
                context.queryType = "insert";
            }
            else if (commandText.ToLower().Contains("delete"))
            {
                context.queryType = "delete";
            }
            else
            {
                context.queryType = "unkown";
            }
            var patterns = new[]
            {
                @"(?i)(from|into|update|delete\s+from)\s+(\w+)"
            };
            var match = Regex.Match(commandText, patterns[0]);
            context.tableName = match.Groups[2].Value;


            Logger.Log($"Query type: {context.queryType}");
            Logger.Log($"Table name: {context.tableName}");
        }

        public async Task VisitPreparingAsync(ExecutionContext context) 
        {
            var commandText = context.Command.CommandText;
            if (commandText.ToLower().Contains("select"))
            {
                context.queryType = "select";
            }
            else if (commandText.ToLower().Contains("update"))
            {
                context.queryType = "update";
            }
            else if (commandText.ToLower().Contains("insert"))
            {
                context.queryType = "insert";
            }
            else if (commandText.ToLower().Contains("delete"))
            {
                context.queryType = "delete";
            }
            else
            {
                context.queryType = "unkown";
            }
            var patterns = new[]
            {
                @"(?i)(from|into|update|delete\s+from)\s+(\w+)"
            };
            var match = Regex.Match(commandText, patterns[0]);
            context.tableName = match.Groups[2].Value;


            Logger.Log($"Query type: {context.queryType}");
            Logger.Log($"Table name: {context.tableName}");
        }
    }

}
