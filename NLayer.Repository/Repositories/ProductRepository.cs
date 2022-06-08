using Microsoft.EntityFrameworkCore;
using Nlayer.Core;
using Nlayer.Core.Repositories;

namespace NLayer.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IPoductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductsWithCategory()
        {
            return await _context.Products.Include(x => x.Category).ToListAsync();
        }
    }
}
