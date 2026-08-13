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
    /// <summary>
    /// Класс был создан для тестирования совместимости с Entity Framework.
    /// Они оказались несовместимы, так что эта часть проекта остаётся в виде демонстрации их несовместимости.
    /// Причина несовместимости в том, что Entity Framework при работе с PostgreSql использует только Npgsql и пытается преобразовывать объекты в него явно.
    /// А NDb-классы из DbGovernator наследуют от абстрактных Db-классов из System.Data.Common, 
    /// что делает их универсальнее и позволяет работать с различными системами управления базами данных, но отнимает возможность использования Entity Framework.
    /// </summary>
    public class ApplicationContext : DbContext
    {
        private NDbDataSource _dataSource;
        public DbSet<Users> users => Set<Users>();
        public DbSet<Accounts> accounts => Set<Accounts>();
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
