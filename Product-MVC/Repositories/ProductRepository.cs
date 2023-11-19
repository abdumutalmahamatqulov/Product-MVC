using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Product_MVC.Data;
using Product_MVC.Entities;

namespace Product_MVC.Repositories;
public class ProductRepository : IProductRepository
{
	private readonly AppDbContext _appDbContext;

	public ProductRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

	public async Task<Product> CreateAudit(Product newProduct, Product oldProduct, string ActionType, User user)
	{
		var auditTrailRecord = new AuditLog
		{
			UserName = user.UserName,
			ControllerName = "Product",
			DateTime = DateTime.UtcNow,
			OldValue = JsonConvert.SerializeObject(oldProduct, Formatting.Indented),
			NewValue = JsonConvert.SerializeObject(newProduct, Formatting.Indented)
		};

		_appDbContext.AuditLogs.Add(auditTrailRecord);
		try
		{
			await _appDbContext.SaveChangesAsync();
			return newProduct;
		}
		catch (Exception ex)
		{
			throw new Exception("Error saving audit log.", ex);
		}
	}

	public async Task<Product> CreateProductAsync(Product product)
	{
		await _appDbContext.Products.AddAsync(product);
		await _appDbContext.SaveChangesAsync();
		return product;
	}

	public async Task<Product> DeleteProductAsync(int id)
	{
		var findProduct = await _appDbContext.Products.FirstOrDefaultAsync(x=>x.Id==id);
		if (findProduct == null) throw new Exception("Product not Found");
		_appDbContext.Products.Remove(findProduct);
		await _appDbContext.SaveChangesAsync();
		return findProduct;
	}

	public async Task<IEnumerable<Product>> GetAllProducts() => await _appDbContext.Products.OrderBy(p => p.Id).ToListAsync();

	public async Task<Product> GetOldValueAsync(int id) => await _appDbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

	public async Task<Product> GetProductByIdAsync(int id)
	{
		var findproduct = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
		return findproduct ?? throw new Exception("Product not Found");
	}

	public async Task<Product> UpdateProductAsync(Product entity)
	{
		var findproduct = await _appDbContext.Products.FirstOrDefaultAsync(i=>i.Id==entity.Id);
		if (findproduct == null) throw new Exception("Product not Found");
		findproduct.Price = entity.Price;
		findproduct.Quantiy = entity.Quantiy;
		findproduct.ItemName = entity.ItemName;
		findproduct.TotalPrice = entity.TotalPrice;
		await _appDbContext.SaveChangesAsync();
		return findproduct;
	}
}
