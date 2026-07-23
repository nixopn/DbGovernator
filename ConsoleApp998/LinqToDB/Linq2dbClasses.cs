using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace DbGovernator.LinqToDB
{
    /// <summary>
    /// Класс, соответствующий таблице пользователей из тестовой базы данных.
    /// </summary>
    [Table(Name = "users")]
    public class User 
    {
        [Column(IsPrimaryKey = true), Identity]
        public int id { get; set; }
        [Column(Name = "name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Класс, соответствующий таблице учётных записей из тестовой базы данных.
    /// </summary>
    [Table(Name = "accounts")]
    public class Account 
    {
        [Column(IsPrimaryKey = true), Identity]
        public int id { get; set; }
        [Column(Name = "money")]
        public int Money { get; set; }
        [Column(Name = "user_id")]
        public int user_id { get; set; }
    }


}
