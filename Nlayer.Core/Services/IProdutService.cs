using Nlayer.Core.DTOs;

namespace Nlayer.Core.Services
{
    public interface IProdutService : IService<Product>
    {
        Task<List<ProductWithCategoryDto>> GetProductsWithCategory();
    }
}
