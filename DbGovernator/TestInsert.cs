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
    /// Проверяет insert-запросы.
    /// </summary>
    internal class TestInsert
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        public string command {  get; set; }

        public void SetupCommand(string command)
        {
            this.command = command;
        }

        /// <summary>
        /// Проверяет асинхронный insert с командой из свойства Command.
        /// </summary>
        /// <returns></returns>
        public async Task InsertQ()
        {
            _logger.LogInfo("Testing insert query");
            _logger.LogInfo("*****************************");
            if(!command.ToLower().Contains("insert"))
            {
                _logger.LogInfo("Invalid command");
                return;
            }
            try
            {
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
            }
            _logger.LogInfo("Test passed");
            _logger.LogInfo("*****************************");
        }

        /// <summary>
        /// Проверяет insert через LinqToDB.
        /// </summary>
        public async Task InsertLinqToDB()
        {
            var dbc = new NDbDataConnectionFactory(_visitors, _logger, _transactionVisitors);
            var db = dbc.CreateConnection();
            var newUser = new User { Name = "AAALinqToDBUser" };
            var insertedId = db.Insert(newUser);
        }
        public TestInsert(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
            _transactionVisitors = transactionVisitors;
        }
    }
}
