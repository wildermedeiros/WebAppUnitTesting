using Microsoft.EntityFrameworkCore;
using System;

namespace WebApp.DatabaseContext
{
    public interface IDbContext : IDisposable
    {
        DbContext Instance { get; }
    }

    public class SalesWebMvcContext : DbContext, IDbContext
    {
        public DbContext Instance => this;
        public SalesWebMvcContext(DbContextOptions<SalesWebMvcContext> options): base(options)
        {
        }
    }

    public static class IDbContextExtensions
    {
        public static DbSet<T> Set<T>(this IDbContext context) where T : class
        {
            return context.Instance.Set<T>();
        }
    }
}
