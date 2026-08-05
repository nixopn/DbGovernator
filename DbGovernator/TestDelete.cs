using Dapper;
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
            _logger.LogInfo("Testing delete query");
            _logger.LogInfo("*****************************");
            if (!command.ToLower().Contains("delete"))
            {
                _logger.LogInfo("Invalid command");
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
                _logger.LogInfo("Test failed");
                _logger.LogInfo(ex.Message);
                _logger.LogInfo("*****************************");
                return;
            }
            _logger.LogInfo("Test passed");
            _logger.LogInfo("*****************************");
        }

        /// <summary>
        /// Проверяет delete через LinqToDB.
        /// </summary>
        public async Task DeleteLinqToDB()
        {
            var dbc = new NDbDataConnectionFactory(_visitors, _logger, _transactionVisitors, _connectionStringProvider);
            var db = dbc.CreateConnection();
            var deleteusers = await db.GetTable<User>().Where(u => u.id == 986).DeleteAsync();
            await db.CloseAsync();
        }

        public async Task DeleteDapper()
        {
            _logger.LogInfo("Testing delete query");
            _logger.LogInfo("*****************************");
            if (!command.ToLower().Contains("delete"))
            {
                _logger.LogInfo("Invalid command");
                return;
            }
            try
            {
                var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
                var NDataSource = NDDataSourceFactory.Create();
                var ndbCon = NDataSource.OpenConnection();
                var sql = "DELETE FROM users WHERE id=@Id";
                var rows = ndbCon.Execute(sql, new { Id = 997 });
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Test failed");
                _logger.LogInfo(ex.Message);
                _logger.LogInfo("*****************************");
                return;
            }
            _logger.LogInfo("Test passed");
            _logger.LogInfo("*****************************");
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
