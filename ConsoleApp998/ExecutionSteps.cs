using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;





namespace ConsoleApp998
{

    internal abstract class ExecutionStep
    {
        public abstract void AcceptVisitor(IVisitor visitor, ExecutionContext context);
    }

    interface IVisitor
    {
        public void VisitPreparing(ExecutionContext context);
        public void VisitBeforeExecution(ExecutionContext context);
        public void VisitExecution(ExecutionContext context);
        public void VisitAfterExecution(ExecutionContext context);
        public void VisitResultProcessing(ExecutionContext context);
    }







    class BeforeExecute : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitBeforeExecution(context);
        }
    }
    class PrepareCommand : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitPreparing(context);
        }
    }
    class AfterExecution : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitAfterExecution(context);
        }
    }
    class ResultProcessing : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitResultProcessing(context);
        }
    }
    class ExecutionSt : ExecutionStep
    {
        public override void AcceptVisitor(IVisitor visitor, ExecutionContext context)
        {
            visitor.VisitExecution(context);
        }
    }






    class Audit : IVisitor
    {
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
            // Поиск имени таблицы доделать
            var patterns = new[]
            {
                @"(?i)(from|into|update|delete\s+from)\s+(\w+)"
            };
            var match =Regex.Match(commandText, patterns[0]);
            context.tableName = match.Groups[2].Value;


            Console.WriteLine($"Query type {context.queryType}");
            Console.WriteLine($"Table name {context.tableName}");
        }
    }





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

    class ExecutionStrategy : IVisitor
    {
        private int _maxRetries;
        private int _retryDelayMs;
        public void VisitAfterExecution(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void VisitBeforeExecution(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void VisitExecution(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public delegate int Operation();

        public void VisitPreparing(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void VisitResultProcessing(ExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class ExecutionContext
    {
        public DbCommand Command { get; set; }

        public DateTime Before { get; set; }
        public DateTime After { get; set; }
        public TimeSpan Duration => After - Before;

        public object? affectedRows { get; set; } 

        public string? tableName { get; set; }
        public string? queryType { get; set; }
    }
}
