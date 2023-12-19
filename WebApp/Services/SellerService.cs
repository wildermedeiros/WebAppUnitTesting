using Microsoft.EntityFrameworkCore;
using WebApp.DatabaseContext;
using WebApp.Models;
using WebApp.Services.Contracts;
using WebApp.Services.Exceptions;

namespace WebApp.Services
{
    public class SellerService : ISellerService
    {
        private readonly IDbContext db;

        public SellerService(IDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Seller>> FindAllAsync()
        {
            return await db.Set<Seller>().ToListAsync();
        }

        public async Task InsertAsync(Seller seller)
        {
            await db.Instance.Set<Seller>().AddAsync(seller);
            await db.Instance.SaveChangesAsync();
            
        }

        public async Task<Seller> FindByIdAsync(int id)
        {
            try 
            {
                return await db.Instance.Set<Seller>()
                    .Include(obj => obj.Department)
                    .FirstOrDefaultAsync(obj => obj.Id == id);
            }
            catch(Exception exception)
            {
                throw new NotFoundException("Id not found");
            }
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await db.Set<Seller>().FindAsync(id);
                db.Set<Seller>().Remove(obj);
                await db.Instance.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException("Can't delete seller because he/she has sales");
            }
        }

        public async Task UpdateAsync(Seller obj)
        {
            bool hasAny = await db.Set<Seller>().AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }
            try
            {
                db.Instance.Update(obj);
                await db.Instance.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
    }
}
