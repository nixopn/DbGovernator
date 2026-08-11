using DbGovernator.NDbClasses;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.LinqToDB
{
    public interface INDbDataConnectionFactory
    {
        public abstract MappingSchema CreateMappingSchema();
        public abstract DataConnection CreateConnection();
        public abstract DataConnection CreateConnection(NDbConnection ndbCon);
        public abstract NDbConnection CreateNDbConnection(NpgsqlConnection npgsqlCon);
    }
}
