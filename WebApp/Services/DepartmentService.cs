using Microsoft.EntityFrameworkCore;
using WebApp.DatabaseContext;
using WebApp.Models;
using WebApp.Services.Contracts;

namespace WebApp.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly WebAppContext _context;

        public DepartmentService(WebAppContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> FindAllAsync()
        {
            return await _context.Set<Department>().OrderBy(x => x.Name).ToListAsync();
        }
    }
}
