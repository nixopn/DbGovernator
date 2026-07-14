using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Контролирует процесс выполнения запроса
    class ExecutionStrategy : IVisitor
    {
        private int _maxRetries; // Максимальное количество попыток повтора в случае провала запроса
        private int _retryDelayMs; // Задержка между попытками

        // Ставит количество попыток
        public void SetTries(int maxRetries)
        {
            _maxRetries = maxRetries;
        }
        // Ставит задержку между попытками
        public void SetRetryDelay(int retryDelayMs)
        {
            _retryDelayMs = retryDelayMs;
        }
        public void VisitAfterExecution(ExecutionContext context)
        {

        }

        public void VisitBeforeExecution(ExecutionContext context)
        {

        }

        // Выполнение запроса
        public void VisitExecution(ExecutionContext context)
        {
            int i = 1;
            do
            {
                try
                {
                    var result = context.executionFunction();
                    context.Result = result.GetType().GetProperty("Result").GetValue(result);
                    // Если результат int в случае ExecuteNonQuery, ExecuteScalar, то записывает в affectedRows
                    if (context.Result is int)
                    {
                        context.affectedRows = (int)context.Result;
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(_retryDelayMs);
                    Console.WriteLine($"Retrying {i} time");
                    if (i == _maxRetries)
                    {
                        throw (new Exception("Service is temporary unavailable"));
                    }
                }
                i++;
            } while (i <= _maxRetries);
            return;
        }


        public void VisitPreparing(ExecutionContext context)
        {

        }

        public void VisitResultProcessing(ExecutionContext context)
        {

        }
    }
}
