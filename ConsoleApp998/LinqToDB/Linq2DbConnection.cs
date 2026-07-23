using DbGovernator.Realisations;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Interceptors;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using DbGovernator.NDbClasses;
using DbGovernator.Abstractions;
using LinqToDB.DataProvider.PostgreSQL;

namespace DbGovernator.LinqToDB
{
    /// <summary>
    /// Класс-наследник DataConnection, реализующий взаимодействие NDb-классов с LinqToDB.
    /// </summary>
    public class  NDbDataConnection : DataConnection
    {
        public NDbDataConnection(IEnumerable<IVisitor> visitors, ILogger logger, IEnumerable<ITransactionVisitor> transactionVisitors)
            : base(new DataOptions()
                  .UsePostgreSQL(PostgreSQLVersion.v15)
                  .UseConnection(new NDbConnectionFactory(visitors, logger, transactionVisitors).CreateConnection()))
        {
            users = this.GetTable<User>();
            accounts = this.GetTable<Account>();
        }


        public ITable<User> users { get; set; }
        public ITable<Account> accounts { get; set; }
    }
}
