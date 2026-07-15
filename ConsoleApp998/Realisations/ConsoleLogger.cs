using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbGovernator.Abstractions;

namespace DbGovernator.Realisations
{
    // Реализация интерфейса ILogger для печати в консоль
    public class ConsoleLogger : ILogger
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
