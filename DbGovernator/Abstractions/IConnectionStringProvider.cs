using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    /// <summary>
    /// Интерфейс, конкретная реализация которого передаёт строку соединения в конкретную реализацию INDbDataSourceFactory
    /// </summary>
    public interface IConnectionStringProvider
    {
        string GetConnectionString();
    }


}
