using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Интерфейс для логирования метрик и аудита по ходу выполнения команд.
    internal interface ILogger
    {
        public string Message { get; set; }
        void Log(string Message);
        void LogWarning(string Message);
        void LogError(string Message);
    }
}
