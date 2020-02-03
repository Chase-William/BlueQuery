using System;
using BlueQueryLibrary;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BlueQueryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1. Get All Giganotosaurus's");
                Console.WriteLine("2. Get All Managarmrs");
                Console.Write("Entry: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        GetAllGiganotosaurus();
                        break;
                    case "2":
                        GetAllManagarmers();
                        break;
                    default:                       
                        break;
                }
            }            
        }

        private static void GetAllManagarmers()
        {
            using var context = new BlueQueryContext(Connect());
            context.Database.EnsureCreated();

            var provider = new BlueprintProvider(context);
            Console.WriteLine("Id       Armor       Fiber       Hide        Chitin");
            foreach (var blueprint in provider.GetAllManagarmr())
            {
                Console.WriteLine(blueprint.ToString());
            }
        }

        private static void GetAllGiganotosaurus()
        {
            using var context = new BlueQueryContext(Connect());
            context.Database.EnsureCreated();

            var provider = new BlueprintProvider(context);
            foreach (var blueprint in provider.GetAllGiganotosaurus())
            {
                Console.WriteLine(blueprint.ToString());
            }
        }

        /// <summary>
        ///     Ultility Function that contains boiler plate code for creating a connection and DbContextOptions for a BlueQueryContext.
        /// </summary>
        private static DbContextOptions<BlueQueryContext> Connect()
        {
            var connection = new SqliteConnection("Data Source=../../../../xUnitBlueQueryLibTest/BlueQueryDB.db");
            connection.Open();

            return new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;
        }
    }
}
