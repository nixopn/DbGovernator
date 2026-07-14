using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Реализация интерфейса ILogger для печати в консоль
    internal class ConsoleLogger : ILogger
    {
        public string Message { get; set; }

        public void Log(string Message)
        {
            Console.WriteLine(Message);
        }


        public void LogWarning(string Message)
        {
        }

        public void LogError(string Message)
        {
        }
    }
}
