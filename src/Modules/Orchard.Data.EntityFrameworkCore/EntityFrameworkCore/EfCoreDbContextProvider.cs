

using Microsoft.EntityFrameworkCore;

namespace Orchard.Data.EntityFrameworkCore
{
    public sealed class EfDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public TDbContext DbContext { get; }

        public EfDbContextProvider(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public TDbContext GetDbContext()
        {
            return DbContext;
        }
    }
}
