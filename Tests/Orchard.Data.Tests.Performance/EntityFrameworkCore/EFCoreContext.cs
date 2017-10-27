using Microsoft.EntityFrameworkCore;

namespace Orchard.Data.Tests.Performance.EntityFrameworkCore
{
    public class EFCoreContext : DbContext
    {
        private readonly string _connectionString;

        public EFCoreContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(_connectionString);

        public DbSet<Post> Posts { get; set; }
    }
}