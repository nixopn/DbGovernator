using DbGovernator.Abstractions;
using LinqToDB;
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
    /// <summary>
    /// Контролирует процесс выполнения запроса.
    /// </summary>
    public class ExecutionStrategy : IVisitor
    {
        public ILogger Logger { get; set; }

        private int _maxRetries = 9; // Максимальное количество попыток повтора в случае провала запроса
        private int _retryDelayMs = 298; // Задержка между попытками


        public ExecutionStrategy()
        {
        }


        public void Reset()
        {
        }
        /// <summary>
        /// Ставит количество попыток.
        /// </summary>
        public void SetTries(int maxRetries)
        {
            _maxRetries = maxRetries;
        }
        /// <summary>
        /// Ставит задержку между попытками.
        /// </summary>
        public void SetRetryDelay(int retryDelayMs)
        {
            _retryDelayMs = retryDelayMs;
        }
        public void VisitAfterExecution(AfterExecution step)
        {

        }

        public void VisitBeforeExecution(BeforeExecute step)
        {

        }

        /// <summary>
        /// Выполнение запроса.
        /// </summary>
        public void VisitExecution(ExecutionSt step)
        {
            int i = 1;
            do
            {
                try
                {
                    var result = step.Context.ExecutionFunction();
                    if(result is int)
                    {
                        step.Context.Result = result;
                        step.Context.AffectedRows = (int)result;
                        return;
                    }
                    if(result is not Task)
                    {
                        step.Context.Result = result;
                        return;
                    }
                    step.Context.Result = result.GetType().GetProperty("Result").GetValue(result);
                    // Если результат int в случае ExecuteNonQuery, ExecuteScalar, то записывает в affectedRows
                    if (step.Context.Result is int)
                    {
                        step.Context.AffectedRows = (int)step.Context.Result;
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
                    Logger.LogInfo($"Retrying {i} time");
                    if (i == _maxRetries)
                    {
                        Logger.LogError(ex.Message);
                        throw new Exception("Service is temporary unavailable", ex);
                    }
                }
                i++;
            } while (i <= _maxRetries);
            return;
        }


        public void VisitPreparing(PrepareCommand step)
        {

        }

        public void VisitResultProcessing(ResultProcessing step)
        {

        }


        public async Task VisitBeforeExecutionAsync(BeforeExecute step) { }

        /// <summary>
        /// Асинхронное выполнение запроса.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task VisitExecutionAsync(ExecutionSt step)
        {
            int i = 1;
            do
            {
                try
                {
                    var TaskObj = step.Context.ExecutionFunction();
                    if(TaskObj is Task task)
                    {
                        await task;
                        var result = task.GetType().GetProperty("Result");
                        if(result != null)
                        {
                            step.Context.Result = result.GetValue(task);
                        }
                    }
                    if (step.Context.Result is int affected)
                    {
                        step.Context.AffectedRows = affected;
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
                    //Thread.Sleep(_retryDelayMs);
                    await Task.Delay(_retryDelayMs);
                    Logger.LogInfo($"Retrying {i} time");
                    if (i == _maxRetries)
                    {
                        Logger.LogError(ex.Message);
                        throw new Exception("Service is temporary unavailable", ex);
                    }
                }
                i++;
            } while (i <= _maxRetries);
            return;
        }

        public async Task VisitAfterExecutionAsync(AfterExecution step) { }

        public async Task VisitResultProcessingAsync(ResultProcessing step) { }
        public async Task VisitPreparingAsync(PrepareCommand step) { }


        public virtual bool HasDbException(Exception ex)
        {
            if(ex == null)
            {
                return false;
            }
            if(ex is DbException || ex is LinqToDBException)
            {
                return true;
            }
            return HasDbException(ex.InnerException);
        }
    }
}
