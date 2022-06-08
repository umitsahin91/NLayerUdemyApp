using Nlayer.Core.DTOs;

namespace Nlayer.Core.Services
{
    public interface ICategoryService : IService<Category>
    {
        Task<CustomResponseDto<CategoryWithProductsDto>> GetSingleCategoryByIdWithProductsAsync(int categoryId);
    }
}
