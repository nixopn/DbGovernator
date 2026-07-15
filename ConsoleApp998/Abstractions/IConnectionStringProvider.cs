using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString();
    }


}
