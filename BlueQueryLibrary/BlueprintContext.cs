﻿using Microsoft.EntityFrameworkCore;
using BlueQueryLibrary.ArkBlueprints;
namespace BlueQueryLibrary
{
    public class BlueprintContext : DbContext
    {
        private readonly string connectionString;

        public BlueprintContext(string _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public BlueprintContext(DbContextOptions<BlueprintContext> options) : base(options) { }

        public DbSet<Blueprint> Blueprints { get; set; }
    }
}

// https://www.codeproject.com/Articles/818694/SQL-Queries-to-Manage-Hierarchical-or-Parent-child
//SELECT childType.blueprint_id, childType.fiber, childType.hide, childType.metal, ParentUserType.armor, SuperType.comment
//FROM Giganotosaurus AS childType
//LEFT JOIN Saddle AS ParentUserType ON childType.blueprint_id = ParentUserType.blueprint_id
//LEFT JOIN Blueprint AS SuperType ON ParentUserType.blueprint_id = SuperType.id;