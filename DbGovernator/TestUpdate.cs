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
            _logger.LogInfo("Testing update query");
            _logger.LogInfo("*****************************");
            if (!command.ToLower().Contains("update"))
            {
                _logger.LogInfo("Invalid command");
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
                _logger.LogInfo("Test failed");
                _logger.LogInfo(ex.Message);
                _logger.LogInfo("*****************************");
                return;
            }
            _logger.LogInfo("Test passed");
            _logger.LogInfo("*****************************");
        }

        /// <summary>
        /// Проверяет update через LinqToDB.
        /// </summary>
        public async Task UpdateLinqToDB()
        {
            var dbc = new NDbDataConnectionFactory(_visitors, _logger, _transactionVisitors, _connectionStringProvider);
            var db = dbc.CreateConnection();
            var update = await db.GetTable<User>().Where(u => u.id == 299).Set(u => u.Name, u => "aaaaaaaa").UpdateAsync();
            await db.CloseAsync();
        }

        public async Task UpdateDapper()
        {
            _logger.LogInfo("Testing update query");
            _logger.LogInfo("*****************************");
            if (!command.ToLower().Contains("update"))
            {
                _logger.LogInfo("Invalid command");
                return;
            }
            try
            {
                var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
                var NDataSource = NDDataSourceFactory.Create();
                var ndbCon = NDataSource.OpenConnection();
                var sql = "UPDATE accounts set money = money + 299 WHERE id = @Id";
                var rows = ndbCon.Execute(sql, new { Id = 9 });
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
        public TestUpdate(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
            _transactionVisitors = transactionVisitors;
        }
    }
}
