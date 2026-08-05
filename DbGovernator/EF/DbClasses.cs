using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.EF
{
    public class Users
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Accounts> accounts { get; set; }
    }

    public class Accounts
    {
        public int id { get; set; }
        public int money { get; set; }
        public int user_id { get; set; }

        public Users User { get; set; }
    }

}
