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
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    /// <summary>
    /// Класс для тестирования DbGovernator.
    /// Проверяет update-запросы.
    /// </summary>
    internal class TestUpdate
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        public string command { get; set; }

        public void SetupCommand(string command)
        {
            this.command = command;
        }

        /// <summary>
        /// Проверяет асинхронный update с командой из свойства Command.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateQ()
        {
            _logger.Log("Testing update query");
            _logger.Log("*****************************");
            if (!command.ToLower().Contains("update"))
            {
                _logger.Log("Invalid command");
                return;
            }
            try
            {
                //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                //var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
                var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
                //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                //var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
                var NDataSource = NDDataSourceFactory.Create();
                await using (var cmd = NDataSource.CreateCommand(command))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Log("Test failed");
                _logger.Log(ex.Message);
                _logger.Log("*****************************");
                return;
            }
            _logger.Log("Test passed");
            _logger.Log("*****************************");
        }

        /// <summary>
        /// Проверяет update через LinqToDB.
        /// </summary>
        public async Task UpdateLinqToDB()
        {
            var db = new NDbDataConnection(_visitors, _logger, _transactionVisitors);
            var update = await db.users.Where(u => u.id == 298).Set(u => u.Name, u => u.Name + "aa").UpdateAsync();
        }
        public TestUpdate(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
            _transactionVisitors = transactionVisitors;
        }
    }
}
