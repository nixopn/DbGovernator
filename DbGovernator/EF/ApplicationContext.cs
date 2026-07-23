using DbGovernator.NDbClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;
using DbGovernator.Realisations;

namespace DbGovernator.EF
{
    public class ApplicationContext : DbContext
    {
        private NDbDataSource _dataSource;
        public DbSet<users> users => Set<users>();
        public DbSet<accounts> accounts => Set<accounts>();
        public ApplicationContext(NDbDataSource ndbDataSource)
        {
            _dataSource = ndbDataSource;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_dataSource);
        }
    }
}
