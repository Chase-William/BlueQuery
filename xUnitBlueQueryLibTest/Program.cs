using BlueQueryLibrary;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BlueQueryLibrary.ArkBlueprints;
using Xunit;

namespace xUnitBlueQueryLibTest
{
    public class Program
    {
        //[Fact]
        //public void MemoryDatabaseTest()
        //{
        //    var connection = new SqliteConnection("Data Source=:memory:");
        //    connection.Open();

        //    var options = new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;

        //    using (var context = new BlueQueryContext(options))
        //    {
        //        context.Database.EnsureCreated();
        //    }

        //    using (var context = new BlueQueryContext(options))
        //    {
        //        context.Blueprints.Add(new Blueprint { Id = 2, Comment = "Test Comment", ImagePath = "N/A" });
        //        context.Blueprints.Add(new Giganotosaurus { Id = 3, Comment = "Test 2 Comment", Metal = 500 });
        //        context.SaveChanges();
        //    }
            


        //    using (var context = new BlueQueryContext(options))
        //    {
        //        var provider = new BlueprintProvider(context);
        //        var blueprint = provider.GetBlueprint(1);
        //        var gigabp = (Giganotosaurus)provider.GetBlueprint(2);

        //        Assert.Equal(500, gigabp.Metal);

        //        Assert.Equal("Test Comment", blueprint.Comment);
        //    }
        //}

        /// <summary>
        ///     PASSING - Add blueprint and retrieve blueprint
        /// </summary>
        //[Fact]
        //public void HardDatabaseTestPassing()
        //{
        //    var connection = new SqliteConnection("Data Source=BlueQueryDB.db");
        //    connection.Open();

        //    var options = new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;

        //    using (var context = new BlueQueryContext(options))
        //    {
        //        context.Database.EnsureCreated();
        //    }

        //    using (var context = new BlueQueryContext(options))
        //    {
        //        context.Blueprints.Add(new Blueprint { Comment = "Auto increment working?", ImagePath = "N/A" });
        //        context.SaveChanges();
        //    }

        //    using (var context = new BlueQueryContext(options))
        //    {
        //        var provider = new BlueprintProvider(context);
        //        var blueprint = provider.GetBlueprint(2);

        //        Assert.Equal("Auto increment working?", blueprint.Comment);                
        //    }
        //}

        [Fact]
        public void TestAddSaddles()
        {
            var connection = new SqliteConnection("Data Source=BlueQueryDB.db");
            connection.Open();

            var options = new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;

            using (var context = new BlueQueryContext(options))
            {
                context.Database.EnsureCreated();
            }

            using (var context = new BlueQueryContext(options))
            {
                context.Saddles.Add(new Giganotosaurus { Comment = "Worst Giga Saddle Ever", Armor = 124, Fiber = 100, Hide = 100, Metal = 100, Discriminator = BlueprintType.Giganotosaurus });
                context.Saddles.Add(new Managarmr { Comment = "A Mana Saddle", Armor = 120, Fiber = 12000, Chitin = 3000, Hide = 2000, Discriminator = BlueprintType.Managarmr });
                context.SaveChanges();
            }

            using (var context = new BlueQueryContext(options))
            {
                var provider = new BlueprintProvider(context);
                var blueprint = provider.GetAllGiganotosaurus();

                Assert.Equal("Worst Giga Saddle Ever", blueprint[0].Comment);
            }
        }
    }
}
