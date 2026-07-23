using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    /// <summary>
    /// Интерфейс для логирования различных событий во время выполнения команд и работы транзакций.
    /// </summary>
    public interface ILogger
    {
        public string Message { get; set; }

        void Log(string Message);
        void LogWarning(string Message);
        void LogError(string Message);
        void LogInfo(string Message);
        void LogDebug(string Message);
    }
}
