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
using LinqToDB;
using System.Linq;
using System.Data.Entity;
using DbGovernator.LinqToDB;


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
            services.AddSingleton<IVisitor, AccessChecker>();
            services.AddSingleton<IVisitor, Metrics>();
            services.AddSingleton<IVisitor, ExecutionStrategy>();
            services.AddSingleton<TestInsert>();
            services.AddSingleton<TestSelect>();
            services.AddSingleton<TestUpdate>();
            services.AddSingleton<TestDelete>();
            services.AddSingleton<TestTransaction>();
            services.AddSingleton<NDbDataSourceFactory>();
            services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddSingleton<NDbConnectionFactory>();
            services.AddSingleton<NDbDataConnection>();
            services.AddSingleton<ITransactionVisitor, TransactionMetrics>();
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var testInsert = serviceProvider.GetService<TestInsert>();
                testInsert.SetupCommand("INSERT INTO users(name) VALUES ('aaaaaaaa');");
                await testInsert.InsertQ();
                var testSelect = serviceProvider.GetRequiredService<TestSelect>();
                testSelect.SetupCommand("SELECT * from users;");
                await testSelect.SelectQ();
                var testUpdate = serviceProvider.GetService<TestUpdate>();
                testUpdate.SetupCommand("UPDATE accounts SET money = money + 300 WHERE user_id = 3;");
                await testUpdate.UpdateQ();
                var testDelete = serviceProvider.GetService<TestDelete>();
                testDelete.SetupCommand("DELETE from users where id = 27");
                await testDelete.DeleteQ();
                var testTransaction = serviceProvider.GetRequiredService<TestTransaction>();
                testTransaction.SetupCommands("UPDATE accounts SET money = money + 200 WHERE user_id = 8", "UPDATE accounts SET money = money + 300 WHERE user_id = 9");
                await testTransaction.TestBasic();
                await testTransaction.TestConflict();
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


                var db = serviceProvider.GetRequiredService<NDbDataConnection>();
                var selected = db.users.Where(u => u.id == 200).ToList();


                foreach (var u in selected)
                {
                    Console.WriteLine($"{u.Name}");
                }


                var deleteusers = await db.users.Where(u => u.id == 997).DeleteAsync();
                var update = await db.users.Where(u => u.id == 289).Set(u => u.Name, u => u.Name + "aa").UpdateAsync();
                var newUser = new User { Name = "AAALinqToDBUser" };
                var insertedId = db.Insert(newUser);

                //INDbDataSourceFactory dataSourceFactory222 = serviceProvider.GetService<INDbDataSourceFactory>();
                //var NdataSource222 = dataSourceFactory222.Create();
                //var ndbcon222 = NdataSource222.OpenConnection();
                //if(ndbcon222 is NDbConnection con222)
                //{
                //    using (var db = new ApplicationContext((NDbDataSource)NdataSource222))
                //    {
                //        db.Database.EnsureCreated();
                //        users u1 = new users() { name = "EF" };
                //        users u2 = new users() { name = "EFF" };
                //        db.users.AddRange(u1, u2);
                //        db.SaveChanges();

                //        var userers = db.users.ToList();
                //        foreach (var u in userers)
                //        {
                //            Console.WriteLine($"{u.id} {u.name}");
                //        }
                //    }
                //}
            }
        }
    }
}


