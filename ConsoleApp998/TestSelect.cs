using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp998
{
    internal class TestSelect
    {
        public string connectionString { get; set; }
        public string command { get; set; }
        public async Task SelectQ()
        {
            if (!command.ToLower().Contains("select"))
            {
                Console.WriteLine("Invlaid command");
                return;
            }
            try
            {
                var dataSource = NpgsqlDataSource.Create(connectionString);
                var NDataSource = new NDbDataSource(dataSource);
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
        public TestSelect(string command, string connectionString)
        {
            this.command = command;
            this.connectionString = connectionString;
        }
    }
}
