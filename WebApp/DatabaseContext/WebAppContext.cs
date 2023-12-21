using Microsoft.EntityFrameworkCore;
using System;
using WebApp.Models;

namespace WebApp.DatabaseContext
{
    public interface IDbContext : IDisposable
    {
        DbContext Instance { get; }

    }

    public class SalesWebMvcContext : DbContext, IDbContext
    {
        virtual public DbContext Instance => this;

        public SalesWebMvcContext(DbContextOptions<SalesWebMvcContext> options): base(options)
        {
        }

        virtual public DbSet<Seller> Seller { get; set; }

    }

    public static class IDbContextExtensions
    {
        public static DbSet<T> Set<T>(this IDbContext context) where T : class
        {
            return context.Instance.Set<T>();
        }
    }
}
