namespace Nlayer.Core.Repositories
{
    public interface IPoductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetProductsWithCategory();
    }
}
