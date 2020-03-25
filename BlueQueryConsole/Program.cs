using System;
using BlueQueryLibrary;
using BlueQueryLibrary.ArkBlueprints;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
/// <summary>
///     
///     The propose of this console program is to provide the developer (me) with a way to interact with the database 
///         without using discord or a DB Browser.
///         
///     This application also serves as a good place for testing along side the xUnit application.
/// 
/// </summary>
namespace BlueQueryConsole
{
    sealed class Program
    {
        private const int OPTIONS_COUNT = 3;

        static void Main(string[] args)
        {
            MainMenu();
        }

        /// <summary>
        ///     The main menu of our application
        /// </summary>
        private static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Get All Blueprints");
                Console.WriteLine("2. Get All Giganotosaurus's");
                Console.WriteLine("3. Get All Managarmrs");
                Console.Write("Entry: ");

                if (int.TryParse(Console.ReadLine(), out int entry) && entry >= 1 && entry <= OPTIONS_COUNT)
                {
                    Console.Clear();
                    var provider = Connect();

                    switch (entry)
                    {
                        case 1:
                            OnGetBlueprints(provider);
                            break;
                        case 2:
                            OnGetGiganotosauruses(provider);
                            break;
                        case 3:
                            OnGetManagarmers(provider);
                            break;
                        default:
                            Console.WriteLine("Error occured after entry validation?!? Entry Value: {0}", entry);
                            break;
                    }
                }
                Console.ReadLine();
            }
        }

        /// <summary>
        ///     Behaves like a handler when the user selects to query all blueprints.
        /// </summary>
        /// <param name="_provider"> Provides the functions needed to query our database. </param>
        private static void OnGetBlueprints(BlueprintProvider _provider)
        {                        
            Console.WriteLine(Blueprint.GetQueryHeader());
            foreach (var bp in _provider.GetBlueprints())
            {
                Console.WriteLine(bp.ToString());
            }
        }

        /// <summary>
        ///     Behaves like a handler when the user selects to query all giganotosaurus blueprints.
        /// </summary>
        /// <param name="_provider"> Provides the functions needed to query our database. </param>
        private static void OnGetGiganotosauruses(BlueprintProvider _provider)
        {
            Console.WriteLine(Giganotosaurus.GetQueryHeader());
            foreach (var bp in _provider.GetGiganotosauruses())
            {
                Console.WriteLine(bp.ToString());
            }
        }

        /// <summary>
        ///     Behaves like a handler when the user selects to query all managarmr blueprints.
        /// </summary>
        /// <param name="_provider"> Provides the functions needed to query our database. </param>
        private static void OnGetManagarmers(BlueprintProvider _provider)
        {
            Console.WriteLine(Managarmr.GetQueryHeader());
            foreach (var bp in _provider.GetManagarmrs())
            {
                Console.WriteLine(bp.ToString());
            }
        }        

        /// <summary>
        ///     Ultility Function that contains boiler plate code for creating a connection and DbContextOptions for a BlueQueryContext.
        /// </summary>
        /// <returns> Returns the provider which raps the bluequerycontext </returns>
        private static BlueprintProvider Connect()
        {
            var connection = new SqliteConnection("Data Source=../../../../BlueQueryDB.db");
            connection.Open();

            var option = new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;

            var context = new BlueQueryContext(option);
            context.Database.EnsureCreated();

            return new BlueprintProvider(context);
        }
    }
}
