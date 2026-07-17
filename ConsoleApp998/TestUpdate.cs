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
            Console.WriteLine("Testing update query");
            Console.WriteLine("*****************************");
            if (!command.ToLower().Contains("update"))
            {
                Console.WriteLine("Invalid command");
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
                Console.WriteLine("Test failed");
                Console.WriteLine(ex.Message);
                Console.WriteLine("*****************************");
                return;
            }
            Console.WriteLine("Test passed");
            Console.WriteLine("*****************************");
        }
        public TestUpdate(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
        }
    }
}
