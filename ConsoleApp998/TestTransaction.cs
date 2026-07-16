using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                //var NDataSource = new NDbDataSource(dataSource, _visitors, Logger);
                var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
                //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                //var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
                var NDataSource = NDDataSourceFactory.Create();
                await using var con = await NDataSource.OpenConnectionAsync();
                await using var transaction = await con.BeginTransactionAsync();
                await using var cmd1 = new NpgsqlCommand(command1);
                await using var cmd11 = new NDbCommand(cmd1, con, transaction, _visitors, _logger);
                await cmd11.ExecuteNonQueryAsync();
                await using var cmd2 = new NpgsqlCommand(command2);
                await using var cmd22 = new NDbCommand(cmd2, con, transaction, _visitors, _logger);
                await cmd22.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed \n {ex.Message}");
                return;
            }
            Console.WriteLine("Test passed");
        }


        // Проверка ситуации конфликта транзакций
        public async Task TestConflict()
        {
            //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
            //var NDataSource = new NDbDataSource(dataSource, _visitors, Logger);
            var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
            //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
            //var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
            var NDataSource = NDDataSourceFactory.Create();
            Task transaction1 = Task.Run(async () =>
            {
                await using var con = await NDataSource.OpenConnectionAsync();
                await using var transaction = await con.BeginTransactionAsync();
                try
                {
                    await using var cmd1 = new NpgsqlCommand(command1);
                    await using var cmd11 = new NDbCommand(cmd1, con, transaction, _visitors, _logger);
                    await cmd11.ExecuteNonQueryAsync();
                    await Task.Delay(1000);
                    await using var cmd2 = new NpgsqlCommand(command2);
                    await using var cmd22 = new NDbCommand(cmd2, con, transaction, _visitors, _logger);
                    await cmd22.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Transaction 1 {ex.Message}");
                    await transaction.RollbackAsync();
                    throw;
                }
            });
            Task transaction2 = Task.Run(async () =>
            {
                await using var con = await NDataSource.OpenConnectionAsync();
                await using var transaction = await con.BeginTransactionAsync();
                try
                {
                    await using var cmd1 = new NpgsqlCommand(command2);
                    await using var cmd11 = new NDbCommand(cmd1, con, transaction, _visitors, _logger);
                    await cmd11.ExecuteNonQueryAsync();
                    await Task.Delay(1000);
                    await using var cmd2 = new NpgsqlCommand(command1);
                    await using var cmd22 = new NDbCommand(cmd2, con, transaction, _visitors, _logger);
                    await cmd22.ExecuteNonQueryAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Transaction 2 {ex.Message} \n");
                    await transaction.RollbackAsync();
                    throw;
                }
            });
            try
            {
                transaction1.Wait();
                transaction2.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed");
                Console.WriteLine($"Error in some transaction {ex.Message}");
                return;
            }
            Console.WriteLine("No conflict");
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
