using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.Realisations;
using DbGovernator.NDbClasses;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;


namespace app
{
    class Programm
    {
        public static async Task Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<INDbDataSourceFactory, NDbDataSourceFactory>();
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton<IVisitor, Audit>();
            services.AddSingleton<IVisitor, Metrics>();
            services.AddSingleton<IVisitor, ExecutionStrategy>();
            services.AddSingleton<TestInsert>();
            services.AddSingleton<TestSelect>();
            services.AddSingleton<TestUpdate>();
            services.AddSingleton<TestDelete>();
            services.AddSingleton<TestTransaction>();
            services.AddSingleton<NDbDataSourceFactory>();
            services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            using (var serviceProvider = services.BuildServiceProvider())
            {
                //var testInsert = serviceProvider.GetService<TestInsert>();
                //testInsert.SetupCommand("INSERT INTO users(name) VALUES ('aaaaaaaa');");
                //await testInsert.InsertQ();
                //var testSelect = serviceProvider.GetRequiredService<TestSelect>();
                //testSelect.SetupCommand("SELECT * from users;");
                //await testSelect.SelectQ();
                //var testUpdate = serviceProvider.GetService<TestUpdate>();
                //testUpdate.SetupCommand("UPDATE accounts SET money = money + 300 WHERE user_id = 3;");
                //await testUpdate.UpdateQ();
                //var testDelete = serviceProvider.GetService<TestDelete>();
                //testDelete.SetupCommand("DELETE from users where id = 27");
                //await testDelete.DeleteQ();
                //var testTransaction = serviceProvider.GetRequiredService<TestTransaction>();
                //testTransaction.SetupCommands("UPDATE accounts SET money = money + 200 WHERE user_id = 8", "UPDATE accounts SET money = money + 300 WHERE user_id = 9");
                //await testTransaction.TestBasic();
                //await testTransaction.TestConflict();
                INDbDataSourceFactory dataSourceFactory = serviceProvider.GetService<INDbDataSourceFactory>();
                var NdataSource = dataSourceFactory.Create();
                Console.WriteLine(NdataSource.GetType().Name);
                var ndbcon = NdataSource.OpenConnection();
                Console.WriteLine(ndbcon.GetType().Name);
                var transaction = ndbcon.BeginTransaction();
                Console.WriteLine(transaction.GetType().Name);
                var ndbcmd = ndbcon.CreateCommand();
                var ndbcmd222 = NdataSource.CreateCommand("INSERT INTO users(name) VALUES ('eodeodeodokeokded');");
                Console.WriteLine(ndbcmd.GetType().Name);
                Console.WriteLine(ndbcmd222.GetType().Name);
                Console.WriteLine(ndbcmd222 is NDbCommand);
                ndbcmd222.ExecuteNonQuery();
                transaction.Rollback();
                ndbcon.Close();

                await using (var cmd = NdataSource.CreateCommand("INSERT INTO users(name) VALUES ('aaaaaaaa');"))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = NdataSource.CreateCommand("INSERT INTO users(name) VALUES ('aaaaaaaa');"))
                {
                    cmd.ExecuteNonQuery();
                }


                using (var cmd = NdataSource.CreateCommand("SELECT * from users where id=229"))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)}");


                    }
                }
                var visitors = serviceProvider.GetRequiredService<IEnumerable<IVisitor>>();
                var logger = serviceProvider.GetRequiredService<ILogger>();
                var dataSource = NpgsqlDataSource.Create(new ConnectionStringProvider().GetConnectionString());
                var DataSourceFactory = new NDbDataSourceFactory(visitors,logger, new ConnectionStringProvider());
                DbDataSource ee = DataSourceFactory.Create();
                Console.WriteLine(ee.GetType().Name);
            }
            DbDataSource ff = NpgsqlDataSource.Create("Host=localhost;Port=5432;Username=postgres;Password=6888;Database=postgres;Pooling=true;MaxPoolSize=2;Timeout=6;");
            DbDataSource ff2 = (DbDataSource)ff;
            var cmddb = ff2.CreateCommand();
            Console.WriteLine(cmddb.GetType().Name);
            var dbcon = ff.OpenConnection();
            Console.WriteLine(dbcon.GetType().Name);
            var npgsqlCmd = ff.CreateCommand();
            Console.WriteLine(npgsqlCmd.GetType().Name);
        }
    }
}


