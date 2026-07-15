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
            if(!command.ToLower().Contains("insert"))
            {
                Console.WriteLine("Invalid command");
                return;
            }
            try
            {
                var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                var NDataSource = new NDbDataSource(dataSource, _visitors, _logger);
                await using (var cmd = NDataSource.CreateCommand(command))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed");
                Console.WriteLine(ex.Message);
                return;
            }
            Console.WriteLine("Test passed");
        }
        public TestInsert(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
        }
    }
}
