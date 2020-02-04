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
        public void TestAddingBlueprints()
        {
            var connection = new SqliteConnection("Data Source=../../../BlueQueryDB.db");
            connection.Open();

            var options = new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;

            using (var context = new BlueQueryContext(options))
            {
                context.Database.EnsureCreated();
            }

            using (var context = new BlueQueryContext(options))
            {
                context.Saddles.Add(new Giganotosaurus { Comment = "Meow", Armor = 134, Fiber = 342, Hide = 352, Metal = 532, Discriminator = BlueprintType.Giganotosaurus });
                context.Saddles.Add(new Managarmr { Comment = "Test", Armor = 100, Fiber = 4325, Chitin = 2945, Hide = 5322, Discriminator = BlueprintType.Managarmr });
                context.Saddles.Add(new Giganotosaurus { Comment = "Giger SAddler", Armor = 124, Fiber = 325, Hide = 947, Metal = 937, Discriminator = BlueprintType.Giganotosaurus });
                context.Saddles.Add(new Managarmr { Comment = "A Mana", Armor = 120.9f, Fiber = 9204, Chitin = 5973, Hide = 9073, Discriminator = BlueprintType.Managarmr });
                context.Saddles.Add(new Giganotosaurus { Comment = "Best Saddle Ever", Armor = 124.7f, Fiber = 532, Hide = 392, Metal = 90, Discriminator = BlueprintType.Giganotosaurus });
                context.Saddles.Add(new Managarmr { Comment = "some bp", Armor = 150.8f, Fiber = 7593, Chitin = 9753, Hide = 9732, Discriminator = BlueprintType.Managarmr });
                context.Saddles.Add(new Giganotosaurus { Comment = "bark", Armor = 124.9f, Fiber = 481, Hide = 937, Metal = 9320, Discriminator = BlueprintType.Giganotosaurus });
                context.Saddles.Add(new Giganotosaurus { Comment = "horrid", Armor = 26.2f, Fiber = 9731, Hide = 123, Metal = 862, Discriminator = BlueprintType.Giganotosaurus });
                context.Saddles.Add(new Managarmr { Comment = "mixer", Armor = 39.2f, Fiber = 5713, Chitin = 7342, Hide = 9382, Discriminator = BlueprintType.Managarmr });
                context.Saddles.Add(new Managarmr { Comment = "just another saddle", Armor = 157.5f, Fiber = 9733, Chitin = 3275, Hide = 1394, Discriminator = BlueprintType.Managarmr });
                context.SaveChanges();
            }

            using (var context = new BlueQueryContext(options))
            {
                var provider = new BlueprintProvider(context);
                var blueprints = provider.GetBlueprints();

                Assert.Equal(10, blueprints.Length);
            }
        }
    }
}
