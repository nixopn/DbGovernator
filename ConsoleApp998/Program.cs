using ConsoleApp998;
using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;


namespace app
{
    class Programm
    {
        public static async Task Main()
        {
            var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=6888;Database=postgres;Pooling=true;MaxPoolSize=2;Timeout=6;";
            TestInsert testins = new TestInsert("INSERT INTO users(name) VALUES ('aaaaaaaa');", connectionString);
            await testins.InsertQ();
            TestSelect test = new TestSelect("SELECT * from users", connectionString);
            await test.SelectQ();
            TestUpdate testupd = new TestUpdate("UPDATE accounts SET money = money + 300 WHERE user_id = 3", connectionString);
            await testupd.UpdateQ();
            TestDelete testdel = new TestDelete("DELETE from users where id = 15", connectionString);
            await testdel.DeleteQ();
            Console.WriteLine("Transaction test № 1");
            TestTransaction testtraction = new TestTransaction("UPDATE accounts SET money = money + 200 WHERE user_id = 8", "UPDATE accounts SET money = money + 300 WHERE user_id = 9", connectionString);
            await testtraction.TestBasic();
            Console.WriteLine("Transaction test № 2");
            await testtraction.TestConflict();


            Console.ReadLine();
            var dataSource = NpgsqlDataSource.Create(connectionString);
            var NdataSource = new NDbDataSource(dataSource);
            using (var cmd = NdataSource.CreateCommand("INSERT INTO users(name) VALUES ('aaaaaaaa');"))
            {
                await cmd.ExecuteNonQueryAsync();
            }


            // "UPDATE users SET name='eeeeeeee' where id=98"
            // "INSERT INTO users(name) VALUES ('aaaaaaaa');"
        }
    }
}


