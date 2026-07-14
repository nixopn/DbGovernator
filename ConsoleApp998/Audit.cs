using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DbGovernator
{
    class Audit : IVisitor
    {
        // Собирает информацию о типе запроса и названии таблицы, на которую он действует
        public void VisitResultProcessing(ExecutionContext context) { }

        public void VisitAfterExecution(ExecutionContext context) { }

        public void VisitBeforeExecution(ExecutionContext context) { }

        public void VisitExecution(ExecutionContext context) { }

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


            Console.WriteLine($"Query type {context.queryType}");
            Console.WriteLine($"Table name {context.tableName}");
        }
    }
}
