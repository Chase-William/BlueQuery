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
        public void Test()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<BlueprintContext>().UseSqlite(connection).Options;

            using (var context = new BlueprintContext(options))
            {
                context.Database.EnsureCreated();
            }

            using (var context = new BlueprintContext(options))
            {
                context.Blueprints.Add(new Blueprint { Id = 1, Comment = "Some comment", ImagePath = "N/" });
                context.SaveChanges();
            }

            using (var context = new BlueprintContext(options))
            {
                var provider = new BlueprintProvider(context);
                var blueprint = provider.GetBlueprint(1);

                Assert.Equal("Some comment", blueprint.Comment);
            }
        }
    }
}
