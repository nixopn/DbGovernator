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
    // Проверяет update-запросы
    internal class TestUpdate
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        public string command { get; set; }

        public void SetupCommand(string command)
        {
            this.command = command;
        }
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
        public TestUpdate(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
        }
    }
}
