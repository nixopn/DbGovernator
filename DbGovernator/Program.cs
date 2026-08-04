using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using LinqToDB;
using System.Linq;
using System.Data.Entity;
using Dapper;

using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.Realisations;
using DbGovernator.NDbClasses;
using DbGovernator.LinqToDB;



namespace app
{
    class Programm
    {
        public static async Task Main()
        {
            //Настройка DI.
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
            services.AddSingleton<NDbDataConnectionFactory>();
            services.AddSingleton<ITransactionVisitor, TransactionMetrics>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                await TestNdb(serviceProvider,
                    "INSERT INTO users(name) VALUES ('aaaaaaaa');",
                    "SELECT * from users;",
                    "UPDATE accounts SET money = money + 300 WHERE user_id = 3;",
                    "DELETE from users where id = 27",
                    "UPDATE accounts SET money = money + 200 WHERE user_id = 8",
                    "UPDATE accounts SET money = money + 300 WHERE user_id = 9"
                    );
                await TestLinqToDB(serviceProvider);
                await TestDapper(serviceProvider);
            }
        }

        public static async Task TestNdb(ServiceProvider serviceProvider, string cmd1, string cmd2, string cmd3, string cmd4, string cmd5, string cmd6)
        {
            var Logger = serviceProvider.GetService<ILogger>();
            Logger.LogInfo("\n *****************************\n  ");
            Logger.LogInfo("Testing basic NDB");
            Logger.LogInfo("\n*****************************\n  ");
            var testInsert = serviceProvider.GetService<TestInsert>();
            testInsert.SetupCommand(cmd1);
            await testInsert.InsertQ();;
            var testSelect = serviceProvider.GetRequiredService<TestSelect>();
            testSelect.SetupCommand(cmd2);
            await testSelect.SelectQ();
            var testUpdate = serviceProvider.GetService<TestUpdate>();
            testUpdate.SetupCommand(cmd3);
            await testUpdate.UpdateQ();
            var testDelete = serviceProvider.GetService<TestDelete>();
            testDelete.SetupCommand(cmd4);
            await testDelete.DeleteQ();
            var testTransaction = serviceProvider.GetRequiredService<TestTransaction>();
            testTransaction.SetupCommands(cmd5, cmd6);
            await testTransaction.TestBasic();
            await testTransaction.TestConflict();
            Logger.LogInfo("\n*****************************\n  ");
        }

        public static async Task TestLinqToDB(ServiceProvider serviceProvider)
        {
            var Logger = serviceProvider.GetService<ILogger>();
            Logger.LogInfo("\n*****************************\n  ");
            Logger.LogInfo("Testing LinqToDB");
            Logger.LogInfo("\n*****************************\n  ");
            var testInsert = serviceProvider.GetService<TestInsert>();
            await testInsert.InsertLinqToDB();
            var testSelect = serviceProvider.GetRequiredService<TestSelect>();
            testSelect.SelectLinqToDB();
            var testUpdate = serviceProvider.GetService<TestUpdate>();
            await testUpdate.UpdateLinqToDB();
            var testDelete = serviceProvider.GetService<TestDelete>();
            await testDelete.DeleteLinqToDB();
            var testTransaction222 = serviceProvider.GetRequiredService<TestTransaction>();
            await testTransaction222.TestBasicLinqToDB();
            await testTransaction222.TestConflictLinqToDB();
            Logger.LogInfo("\n*****************************\n  ");
        }

        public static async Task TestDapper(ServiceProvider serviceProvider)
        {
            var Logger = serviceProvider.GetService<ILogger>();
            Logger.LogInfo("\n*****************************\n  ");
            Logger.LogInfo("Testing Dapper");
            Logger.LogInfo("\n*****************************\n  ");
            var testInsert = serviceProvider.GetService<TestInsert>();
            await testInsert.InsertDapper();
            var testSelect = serviceProvider.GetRequiredService<TestSelect>();
            testSelect.SelectDapper();
            var testUpdate = serviceProvider.GetService<TestUpdate>();;
            await testUpdate.UpdateDapper();
            var testDelete = serviceProvider.GetService<TestDelete>();
            await testDelete.DeleteDapper();
            var testTransaction222 = serviceProvider.GetRequiredService<TestTransaction>();
            await testTransaction222.TestBasicDapper();
            await testTransaction222.TestConflictDapper();
            Logger.LogInfo("\n*****************************\n  ");
        }
    }
}


