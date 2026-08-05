using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Interceptors;
using LinqToDB.Mapping;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.LinqToDB
{
    /// <summary>
    /// Класс-наследник DataConnection, реализующий взаимодействие NDb-классов с LinqToDB.
    /// </summary>
    public class  NDbDataConnectionFactory
    {
        private MappingSchema _mappingSchema;
        private IEnumerable<IVisitor> _visitors;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        public NDbDataConnectionFactory(IEnumerable<IVisitor> visitors, 
            ILogger logger, 
            IEnumerable<ITransactionVisitor> transactionVisitors, 
            IConnectionStringProvider connectionStringProvider)
        {
            _visitors = visitors;
            _logger = logger;
            _transactionVisitors = transactionVisitors;
            _mappingSchema = CreateMappingSchema();
            _connectionStringProvider = connectionStringProvider;
        }

        private MappingSchema CreateMappingSchema()
        {
            var schema = new MappingSchema();
            var builder = new FluentMappingBuilder(schema);
            builder.Entity<User>()
                .HasTableName("users")
                .Property(x => x.id).IsPrimaryKey().IsIdentity()
                .Property(x => x.Name).HasColumnName("name");
            builder.Entity<Account>()
                .HasTableName("users")
                .Property(x => x.id).IsPrimaryKey().IsIdentity()
                .Property(x => x.Money).HasColumnName("money")
                .Property(x => x.user_id).HasColumnName("user_id");
            builder.Build();
            return schema;
        }

        public DataConnection CreateConnection()
        {
            var npgsqlCon = new NpgsqlConnection(_connectionStringProvider.GetConnectionString());
            var ndbCon = new NDbConnection(npgsqlCon, _visitors, _logger, _transactionVisitors);
            var dataConnection = new DataConnection(new DataOptions()
                .UsePostgreSQL(PostgreSQLVersion.v15)
                .UseConnection(ndbCon)
                .UseMappingSchema(_mappingSchema)
            );
            return dataConnection;

        }
    }
}
