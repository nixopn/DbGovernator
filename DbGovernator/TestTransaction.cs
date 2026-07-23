using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.LinqToDB;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using LinqToDB;
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
    /// <summary>
    /// Класс для тестирования DbGovernator.
    /// Проверяет работу транзакций.
    /// </summary>
    internal class TestTransaction
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        public string connectionString { get; set; }
        public string command1 { get; set; }
        public string command2 { get; set; }

        /// <summary>
        /// Проверка базовой транзакции.
        /// </summary>
        /// <returns></returns>
        public async Task TestBasic()
        {
            _logger.LogInfo("Testing basic transaction");
            _logger.LogInfo("*****************************");
            try
            {
                var NDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider, _transactionVisitors);
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
                _logger.LogInfo($"Test failed \n {ex.Message}");
                _logger.LogInfo("*****************************");
                return;
            }
            _logger.LogInfo("Test passed");
            _logger.LogInfo("*****************************");
        }

        /// <summary>
        /// Проверка ситуации конфликта транзакций.
        /// </summary>
        /// <returns></returns>
        public async Task TestConflict()
        {
            _logger.LogInfo("Testing conflict of transactions");
            _logger.LogInfo("*****************************");
            var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider, _transactionVisitors);
            var NDataSource = NDDataSourceFactory.Create();
            Task transaction1 = Task.Run((Func<Task?>)(async () =>
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

                        _logger.LogInfo($"Transaction 1 {ex.Message}");
                        await transaction.RollbackAsync();
                        _logger.LogInfo("*****************************");
                        throw;
                    }
                }
            }));
            Task transaction2 = Task.Run((Func<Task?>)(async () =>
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

                        _logger.LogInfo($"Transaction 2 {ex.Message}");
                        await transaction.RollbackAsync();
                        _logger.LogInfo("*****************************");
                        throw;
                    }
                }
            }));
            try
            {
                await Task.WhenAll(transaction1, transaction2);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Test failed");
                _logger.LogInfo($"Error in some transaction {ex.Message}");
                _logger.LogInfo("*****************************");
                return;
            }
            _logger.LogInfo("No conflict");
            _logger.LogInfo("*****************************");
        }

        /// <summary>
        /// Проверка базовой транзакции через LinqToDB.
        /// </summary>
        /// <returns></returns>
        public async Task TestBasicLinqToDB()
        {
            _logger.LogInfo("Testing basic transaction LinqToDB");
            _logger.LogInfo("*****************************");
            var dbc = new NDbDataConnectionFactory(_visitors, _logger, _transactionVisitors);
            var db = dbc.CreateConnection();
            using (var trs = await db.BeginTransactionAsync())
            {
                try
                {
                    var update = await db.GetTable<Account>().Where(u => u.id == 8).Set(u => u.Money, u => u.Money + 200).UpdateAsync();
                    var update2 = await db.GetTable<Account>().Where(u => u.id == 9).Set(u => u.Money, u => u.Money + 300).UpdateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogInfo($"Test failed \n {ex.Message}");
                    _logger.LogInfo("*****************************");
                    return;
                }
            }


            _logger.LogInfo("Test passed");
            _logger.LogInfo("*****************************");
        }

        /// <summary>
        /// Проверка ситуации конфликта транзакций через LinqToDB.
        /// </summary>
        /// <returns></returns>
        public async Task TestConflictLinqToDB()
        {
            _logger.LogInfo("Testing conflict of transactions LinqToDB");
            _logger.LogInfo("*****************************");
            var dbc = new NDbDataConnectionFactory(_visitors, _logger, _transactionVisitors);
            var db = dbc.CreateConnection();
            Task transaction1 = Task.Run((Func<Task?>)(async () =>
            {
                            using (var trs = await db.BeginTransactionAsync())
            {
                try
                {
                    var update = await db.GetTable<Account>().Where(u => u.id == 8).Set(u => u.Money, u => u.Money + 200).UpdateAsync();
                    var update2 = await db.GetTable<Account>().Where(u => u.id == 9).Set(u => u.Money, u => u.Money + 300).UpdateAsync();
                }
                catch (Exception ex)
                {
                        _logger.LogInfo($"Transaction №1 failed \n {ex.Message}");
                        _logger.LogInfo("*****************************");
                    return;
                }
            }
            }));
            Task transaction2 = Task.Run((Func<Task?>)(async () =>
            {
                using (var trs = await db.BeginTransactionAsync())
                {
                    try
                    {
                        var update = await db.GetTable<Account>().Where(u => u.id == 9).Set(u => u.Money, u => u.Money + 300).UpdateAsync();
                        var update2 = await db.GetTable<Account>().Where(u => u.id == 8).Set(u => u.Money, u => u.Money + 200).UpdateAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInfo($"Transaction №2 failed \n {ex.Message}");
                        _logger.LogInfo("*****************************");
                        return;
                    }
                }
            }));
            try
            {
                await Task.WhenAll(transaction1, transaction2);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Test failed");
                _logger.LogInfo($"Error in some transaction {ex.Message}");
                _logger.LogInfo("*****************************");
                return;
            }
            _logger.LogInfo("No conflict");
            _logger.LogInfo("*****************************");
        }

        /// <summary>
        /// Устанавливает значения команд для тестов транзакций.
        /// </summary>
        /// <param name="command1"></param>
        /// <param name="command2"></param>
        public void SetupCommands(string command1, string command2)
        {
            this.command1 = command1;
            this.command2 = command2;
        }


        public TestTransaction(IConnectionStringProvider connectionStringProvider, IEnumerable<IVisitor> visitors, ILogger logger, IEnumerable<ITransactionVisitor> transactionVisitors) 
        { 
            _connectionStringProvider = connectionStringProvider;
            _visitors = visitors;
            _logger = logger;
            _transactionVisitors = transactionVisitors;
        }
    }
}
