using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace DbGovernator
{
    public class NDbConnectionFactory
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        IEnumerable<ITransactionVisitor> _transactionVisitors;
        public NDbConnectionFactory(IEnumerable<IVisitor> visitors, ILogger logger, IEnumerable<ITransactionVisitor> transactionVisitors) 
        {
            _visitors = visitors;
            _logger = logger;
            _transactionVisitors = transactionVisitors;
        }
        public DbConnection CreateConnection()
        {
            var npgsqlCon = new NpgsqlConnection(new ConnectionStringProvider().GetConnectionString());
            var ndbCon = new NDbConnection(npgsqlCon, _visitors, _logger, _transactionVisitors);
            return ndbCon;
        }
    }
}
