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
    // Проверяет insert-запросы
    internal class TestInsert
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger _logger;
        private IConnectionStringProvider _connectionStringProvider;
        public string command {  get; set; }

        public void SetupCommand(string command)
        {
            this.command = command;
        }
        public async Task InsertQ()
        {
            _logger.Log("Testing insert query");
            _logger.Log("*****************************");
            if(!command.ToLower().Contains("insert"))
            {
                _logger.Log("Invalid command");
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
                _logger.Log("Test failed");
                _logger.Log(ex.Message);
                _logger.Log("*****************************");
            }
            _logger.Log("Test passed");
            _logger.Log("*****************************");
        }
        public TestInsert(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
        }
    }
}
