using System;
using System.Collections.Generic;
using DbGovernator.NDbClasses;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    public interface INDbDataSourceFactory
    {
        public DbDataSource Create();
    }
}
