using Domain;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext: DbContext
    {
        private static string ConnectionString =
            "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_ktamma_battleship_01;MultipleActiveResultSets=true";

        
        
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<Config> Configs { get; set; } = default!;
        public DbSet<Ship> Ships { get; set; } = default!;
        public DbSet<ShipConfig> ShipConfigs { get; set; } = default!;
        public DbSet<TouchRule> TouchRules { get; set; } = default!;


        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // remove the cascade delete globally
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        
    }
}