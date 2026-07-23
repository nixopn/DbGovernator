using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Realisations
{
    /// <summary>
    /// Реализация интерфейса провайдера строки соединения.
    /// Даёт строку соединения.
    /// </summary>
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        public string GetConnectionString()
        {
            return "Host=localhost;Port=5432;Username=postgres;Password=6888;Database=postgres;Pooling=true;MaxPoolSize=9;Timeout=6;";
        }
    }
}
