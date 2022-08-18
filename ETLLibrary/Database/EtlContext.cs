using ETLLibrary.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ETLLibrary.Database
{
    public class EtlContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Csv> CsvFiles { get; set; }
        public DbSet<DbPipeline> DbPipelines { get; set; }
        public DbSet<DbConnection> DbConnections { get; set; }
        private const string ConnectionString = "Server=.; Database=ETLDB; Trusted_Connection=True;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Csv>()
                .HasOne(p => p.User)
                .WithMany(b => b.CsvFiles)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<DbConnection>()
                .HasOne(p => p.User)
                .WithMany(b => b.DbConnections)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<DbPipeline>()
                .HasOne(p => p.User)
                .WithMany(b => b.DbPipelines)
                .HasForeignKey(p => p.UserId);
        }
    }
}