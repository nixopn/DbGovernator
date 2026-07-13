using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp998
{
    internal class TestInsert
    {
        public string connectionString {  get; set; }
        public string command {  get; set; }
        public async Task InsertQ()
        {
            if(!command.ToLower().Contains("insert"))
            {
                Console.WriteLine("Invalid command");
                return;
            }
            try
            {
                var dataSource = NpgsqlDataSource.Create(connectionString);
                var NDataSource = new NDbDataSource(dataSource);
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
        public TestInsert(string command, string connectionString)
        {
            this.command = command;
            this.connectionString = connectionString;
        }
    }
}
