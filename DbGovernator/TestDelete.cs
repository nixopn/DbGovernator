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
    /// Проверяет delete-запросы.
    /// </summary>
    internal class TestDelete
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
        /// Проверяет асинхронный delete с командой из свойства Command.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteQ()
        {
            _logger.Log("Testing delete query");
            _logger.Log("*****************************");
            if (!command.ToLower().Contains("delete"))
            {
                _logger.Log("Invalid command");
                return;
            }
            try
            {
                var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
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
        /// Проверяет delete через LinqToDB.
        /// </summary>
        public async Task DeleteLinqToDB()
        {
            var dbc = new NDbDataConnectionFactory(_visitors, _logger, _transactionVisitors);
            var db = dbc.CreateConnection();
            var deleteusers = await db.GetTable<User>().Where(u => u.id == 997).DeleteAsync();
        }
        public TestDelete(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
            _transactionVisitors = transactionVisitors;
        }
    }
}
