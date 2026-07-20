using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Класс для тестирования DbGovernator
    // Проверяет работу транзакций
    internal class TestTransaction
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        public string connectionString { get; set; }
        public string command1 { get; set; }
        public string command2 { get; set; }


        // Проверка базовой транзакции
        public async Task TestBasic()
        {
            Console.WriteLine("Testing basic transaction");
            Console.WriteLine("*****************************");
            try
            {
                var NDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
                var NDataSource = NDataSourceFactory.Create();
                using (var con = await NDataSource.OpenConnectionAsync())
                using (var trs = await con.BeginTransactionAsync())
                    {
                        using (var cmd1 = con.CreateCommand())
                        {
                            cmd1.CommandText = command1;
                            await cmd1.ExecuteNonQueryAsync();
                        }
                        await Task.Delay(1000);
                        using (var cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = command2;
                            await cmd2.ExecuteNonQueryAsync();
                        }
                        await trs.CommitAsync();
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed \n {ex.Message}");
                Console.WriteLine("*****************************");
                return;
            }
            Console.WriteLine("Test passed");
            Console.WriteLine("*****************************");
        }


        // Проверка ситуации конфликта транзакций
        public async Task TestConflict()
        {
            Console.WriteLine("Testing conflict of transactions");
            Console.WriteLine("*****************************");
            //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
            //var NDataSource = new NDbDataSource(dataSource, _visitors, Logger);
            var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
            //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
            //var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
            var NDataSource = NDDataSourceFactory.Create();
            Task transaction1 = Task.Run(async () =>
            {
                using (var con = await NDataSource.OpenConnectionAsync())
                using (var transaction = await con.BeginTransactionAsync())
                {
                    try
                    {
                        await using (var cmd1 = con.CreateCommand())
                        {
                            cmd1.CommandText = command1;
                            await cmd1.ExecuteNonQueryAsync();
                        }
                        await Task.Delay(1000);
                        await using (var cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = command2;
                            await cmd2.ExecuteNonQueryAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"Transaction 1 {ex.Message}");
                        await transaction.RollbackAsync();
                        Console.WriteLine("*****************************");
                        return;
                    }
                }
            });
            Task transaction2 = Task.Run(async () =>
            {
                await using (var con = await NDataSource.OpenConnectionAsync())
                await using (var transaction = await con.BeginTransactionAsync())
                {
                    try
                    {
                        await using (var cmd1 = con.CreateCommand())
                        {
                            cmd1.CommandText = command2;
                            await cmd1.ExecuteNonQueryAsync();
                        }
                        await Task.Delay(1000);
                        await using (var cmd2 = con.CreateCommand())
                        {
                            cmd2.CommandText = command1;
                            await cmd2.ExecuteNonQueryAsync();
                        }
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"Transaction 2 {ex.Message}");
                        await transaction.RollbackAsync();
                        Console.WriteLine("*****************************");
                        return;
                    }
                }
            });
            try
            {
                await Task.WhenAll(transaction1, transaction2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed");
                Console.WriteLine($"Error in some transaction {ex.Message}");
                Console.WriteLine("*****************************");
                return;
            }
            Console.WriteLine("No conflict");
            Console.WriteLine("*****************************");
        }



        public void SetupCommands(string command1, string command2)
        {
            this.command1 = command1;
            this.command2 = command2;
        }


        public TestTransaction(IConnectionStringProvider connectionStringProvider, IEnumerable<IVisitor> visitors, ILogger logger) 
        { 
            _connectionStringProvider = connectionStringProvider;
            _visitors = visitors;
            _logger = logger;
        }
    }
}
