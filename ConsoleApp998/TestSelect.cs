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
    // Проверяет select-запросы и ExecuteReader
    internal class TestSelect
    {
        private IEnumerable<IVisitor> _visitors;
        private ILogger Logger;
        private IConnectionStringProvider _connectionStringProvider;
        public string command { get; set; }
        public void SetupCommand(string command)
        {
            this.command = command;
        }

        public async Task SelectQ()
        {
            if (!command.ToLower().Contains("select"))
            {
                Console.WriteLine("Invlaid command");
                return;
            }
            try
            {
                var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
                var NDataSource = new NDbDataSource(dataSource, _visitors, Logger);
                await using (var cmd = NDataSource.CreateCommand(command))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)}");

                    }
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
        public TestSelect(IConnectionStringProvider connectionStringProvider, IEnumerable<IVisitor> visitors, ILogger logger)
        {
            this._connectionStringProvider= connectionStringProvider;
            _visitors = visitors;
            Logger = logger;
        }
    }
}
