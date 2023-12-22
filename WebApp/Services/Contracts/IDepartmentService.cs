using WebApp.Models;

namespace WebApp.Services.Contracts
{
    public interface IDepartmentService
    {
        public Task<List<Department>> FindAllAsync();
    }
}
