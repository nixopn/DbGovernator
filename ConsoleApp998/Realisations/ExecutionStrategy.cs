using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    // Контролирует процесс выполнения запроса
    public class ExecutionStrategy : IVisitor
    {
        public bool hadException { get; set; }
        public ILogger Logger { get; set; }

        private int _maxRetries = 9; // Максимальное количество попыток повтора в случае провала запроса
        private int _retryDelayMs = 298; // Задержка между попытками


        public ExecutionStrategy()
        {
            hadException = false;
        }


        public void Reset()
        {
            hadException = false;
        }
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
                    if(result is int)
                    {
                        context.Result = result;
                        context.affectedRows = (int)result;
                        return;
                    }
                    if(result is not Task)
                    {
                        context.Result = result;
                        return;
                    }
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
                    if (!HasDbException(ex))
                    {
                        throw new Exception("Not an sql exception", ex);
                    }
                    // Console.WriteLine(ex.Message);
                    Thread.Sleep(_retryDelayMs);
                    Logger.Log($"Retrying {i} time");
                    if (i == _maxRetries)
                    {
                        throw new Exception("Service is temporary unavailable", ex);
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


        private bool HasDbException(Exception ex)
        {
            if(ex == null)
            {
                return false;
            }
            if(ex is DbException)
            {
                return true;
            }
            return HasDbException(ex.InnerException);
        }
    }
}
