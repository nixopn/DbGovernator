using System;
using System.Collections.Generic;
using DbGovernator.NDbClasses;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    /// <summary>
    /// Абстрактный интерфейс, конкретные реализации которого создают экземпляры класса NDbDataSource.
    /// </summary>
    public interface INDbDataSourceFactory
    {
        public DbDataSource Create();
    }
}
