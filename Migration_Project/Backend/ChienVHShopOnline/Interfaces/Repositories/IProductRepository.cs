using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Interfaces
{
    public interface IProductRepository : IRepository<int, Product>
    {
        Task<IEnumerable<Product>> GetPagedProducts(int pageNumber, int pageSize);
        Task<IEnumerable<Product>> GetPagedProductsByCategory(int categoryId, int pageNumber, int pageSize);
        Task<IEnumerable<Product>> GetPagedProductsByUserId(int userId, int pageNumber, int pageSize);
    }
}
