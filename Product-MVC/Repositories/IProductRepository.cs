using Product_MVC.Dto_s;
using Product_MVC.Entities;

namespace Product_MVC.Repositories;

public interface IProductRepository
{
	public Task<Product>GetProductByIdAsync(int id);
	public Task<IEnumerable<Product>>GetAllProducts();
	public Task<Product> CreateProductAsync(Product product);
	public Task<Product> UpdateProductAsync(Product entity);
	public Task<Product> DeleteProductAsync(int id);
	public Task<Product> GetOldValueAsync(int id);
	public Task<Product> CreateAudit(Product newProduct,Product oldProduct, string ActionType, User user);
}
