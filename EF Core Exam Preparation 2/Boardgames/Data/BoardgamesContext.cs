﻿namespace Boardgames.Data
{
    using Boardgames.Data.Models;
    using Microsoft.EntityFrameworkCore;
    
    public class BoardgamesContext : DbContext
    {
        public BoardgamesContext()
        { 
        }

        public BoardgamesContext(DbContextOptions options)
            : base(options) 
        {
        }

        public DbSet<Boardgame> Boardgames  { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Creator> Creators { get; set; }
        public DbSet<BoardgameSeller> BoardgamesSellers  { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BoardgameSeller>(e =>
                e.HasKey(bs => new { bs.BoardgameId, bs.SellerId }));
        }
    }
}
