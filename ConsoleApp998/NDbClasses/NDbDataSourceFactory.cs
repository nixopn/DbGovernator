using DbGovernator.Abstractions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.NDbClasses
{
    public class NDbDataSourceFactory : INDbDataSourceFactory
    {
        IEnumerable<IVisitor> _visitors;
        ILogger _logger;
        IConnectionStringProvider _connectionStringProvider;
        public NDbDataSourceFactory(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
        }
        public DbDataSource Create()
        {
            var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
            var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
            return NDataSource;
        }
    }
}
