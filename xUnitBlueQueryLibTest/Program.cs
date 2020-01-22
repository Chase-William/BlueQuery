using BlueQueryLibrary;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BlueQueryLibrary.ArkBlueprints;
using Xunit;

namespace xUnitBlueQueryLibTest
{
    public class Program
    {
        [Fact]
        public void MemoryDatabaseTest()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<BlueQueryContext>().UseSqlite(connection).Options;

            using (var context = new BlueQueryContext(options))
            {
                context.Database.EnsureCreated();
            }

            using (var context = new BlueQueryContext(options))
            {
                context.Blueprints.Add(new Blueprint { Id = 1, Comment = "Test Comment", ImagePath = "N/A" });
                context.Blueprints.Add(new Giganotosaurus { Id = 2, Comment = "Test 2 Comment", Metal = 500 });
                context.SaveChanges();
            }
            


            using (var context = new BlueQueryContext(options))
            {
                var provider = new BlueprintProvider(context);
                var blueprint = provider.GetBlueprint(1);
                var gigabp = (Giganotosaurus)provider.GetBlueprint(2);

                Assert.Equal(500, gigabp.Metal);

                Assert.Equal("Test Comment", blueprint.Comment);
            }
        }

        [Fact]
        public void FileDatabaseTest()
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
                context.Blueprints.Add(new Blueprint { Id = 1, Comment = "Test Comment", ImagePath = "N/A" });
                //context.Blueprints.Add(new Giganotosaurus { Id = 2, Comment = "Test 2 Comment", Metal = 500 });
                context.SaveChanges();
            }



            using (var context = new BlueQueryContext(options))
            {
                var provider = new BlueprintProvider(context);
                var blueprint = provider.GetBlueprint(1);
                //var gigabp = (Giganotosaurus)provider.GetBlueprint(2);

               // Assert.Equal(500, gigabp.Metal);

                Assert.Equal("Test Comment", blueprint.Comment);
            }
        }
    }
}
