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
                Console.WriteLine("1. Query All Blueprints");
                Console.WriteLine("Entry: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        var connection = new SqliteConnection("Data Source=../../../../xUnitBlueQueryLibTest/BlueQueryDB.db");
                        connection.Open();

                        var options = new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;

                        using (var context = new BlueQueryContext(options))
                        {
                            context.Database.EnsureCreated();

                            var provider = new BlueprintProvider(context);
                            foreach (var blueprint in provider.GetAllGiganotosaurus())
                            {
                                Console.WriteLine(blueprint.ToString());
                            }
                        }                        
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
