using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.LinqToDB;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
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
    /// Проверяет select-запросы и ExecuteReader.
    /// </summary>
    public class TestSelect
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        private IEnumerable<ITransactionVisitor> _transactionVisitors;
        public string Command { get; set; }
        public void SetupCommand(string command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Проверяет асинхронный select с командой из свойства Command.
        /// </summary>
        /// <returns></returns>
        public async Task SelectQ()
        {
            _logger.Log("Testing select query");
            _logger.Log("*****************************");
            if (!Command.ToLower().Contains("select"))
            {
                _logger.Log("Invlaid command");
                return;
            }
            try
            {
                //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                //var NDataSource = new NDbDataSource(dataSource, _visitors, Logger);
                var NDDataSourceFactory = new NDbDataSourceFactory(_visitors, _logger, _connectionStringProvider);
                //var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                //var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
                var NDataSource = NDDataSourceFactory.Create();
                await using (var cmd = NDataSource.CreateCommand(Command))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        _logger.Log($"{reader.GetInt32(0)} {reader.GetString(1)}");

                    }
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
        /// Проверяет select через LinqToDB.
        /// </summary>
        public void SelectLinqToDB()
        {
            var dbc = new NDbDataConnectionFactory(_visitors, _logger, _transactionVisitors);
            var db = dbc.CreateConnection();
            var selected = db.GetTable<User>().Where(u => u.id >= 200).ToList();


            foreach (var u in selected)
            {
                Console.WriteLine($"{u.Name}");
            }
        }
        public TestSelect(IConnectionStringProvider connectionStringProvider, IEnumerable<IVisitor> visitors, ILogger logger, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            this._connectionStringProvider= connectionStringProvider;
            _visitors = visitors;
            _logger = logger;
            _transactionVisitors = transactionVisitors;
        }
    }
}
