using Microsoft.EntityFrameworkCore;
using BlueQueryLibrary.Blueprints;
using BlueQueryLibrary.Blueprints.CustomBlueprints;

namespace BlueQueryLibrary
{
    public class BlueQueryContext : DbContext
    {
        private readonly string connectionString;
        public DbSet<Blueprint> Saddles { get; set; }
        
        public BlueQueryContext(string _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public BlueQueryContext(DbContextOptions<BlueQueryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blueprint>()
                .HasDiscriminator<int>("discriminator")
                .HasValue<Giganotosaurus>(0)
                .HasValue<Managarmr>(1);
        }
    }
}

// https://www.codeproject.com/Articles/818694/SQL-Queries-to-Manage-Hierarchical-or-Parent-child
//SELECT childType.blueprint_id, childType.fiber, childType.hide, childType.metal, ParentUserType.armor, SuperType.comment
//FROM Giganotosaurus AS childType
//LEFT JOIN Saddle AS ParentUserType ON childType.blueprint_id = ParentUserType.blueprint_id
//LEFT JOIN Blueprint AS SuperType ON ParentUserType.blueprint_id = SuperType.id;