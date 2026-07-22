using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    public class TransactionContext
    {
        public bool Success { get; set; }
        public DateTime Before { get; set; }
        public DateTime After { get; set; }
        public TimeSpan Duration => After - Before;
    }
}
