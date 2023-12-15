using WebApp.Models;

namespace WebApp.Services.Contracts
{
    public interface ISellerService
    {
        public Task<List<Seller>> FindAllAsync();
        public Task InsertAsync(Seller obj);
        public Task<Seller> FindByIdAsync(int id);
        public Task RemoveAsync(int id);
        public Task UpdateAsync(Seller obj);
    }
}
