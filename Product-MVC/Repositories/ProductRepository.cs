using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Product_MVC.Data;
using Product_MVC.Entities;

namespace Product_MVC.Repositories;
public class ProductRepository : IProductRepository
{
	private readonly AppDbContext _appDbContext;

	public ProductRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

	public async Task<Product> CreateAudit(Product newProduct, Product oldProduct, string actionType, User user)
	{
		var auditTrailRecord = new AuditLog
		{
			UserName = user.UserName,
			Action=actionType,
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
		_appDbContext.Products.Add(product);
		await _appDbContext.SaveChangesAsync();
		return product;
	}

	public async Task<Product> DeleteProductAsync(int id)
	{
	var getid =  await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
		_appDbContext.Products.Remove(getid);
		await _appDbContext.SaveChangesAsync();
		return getid;
	}

	public async Task<IEnumerable<Product>> GetAllProducts()
	{
		return await _appDbContext.Products.ToListAsync();
	}

	public async Task<Product> GetOldValueAsync(int id) => await _appDbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);



	public async Task<Product> GetProductByIdAsync(int id)
	{
		return await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<Product> UpdateProductAsync(Product product)
	{
		var findproduct = await _appDbContext.Products.FirstOrDefaultAsync(i => i.Id == product.Id);
		findproduct.Price = product.Price;
		findproduct.Quantiy = product.Quantiy;
		findproduct.ItemName = product.ItemName;
		findproduct.TotalPrice = product.TotalPrice;
		_appDbContext.Products.Update(findproduct);
		await _appDbContext.SaveChangesAsync();
		return findproduct;
	}
}
