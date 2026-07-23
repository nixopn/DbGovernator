using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.EF
{
    public class users
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<accounts> accounts { get; set; }
    }

    public class accounts
    {
        public int id { get; set; }
        public int money { get; set; }
        public int user_id { get; set; }

        public users User { get; set; }
    }

}
