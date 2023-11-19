using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_MVC.Data;
using Product_MVC.Dto_s;
using Product_MVC.Entities;

namespace Product_MVC.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _appDbContext;

    public ProductController(AppDbContext appDbContext) => _appDbContext = appDbContext;
    public async Task<IActionResult> table()
    {
    
        var indexproduct = await _appDbContext.Products.ToListAsync();
        return View("table",indexproduct);
    }
	public async Task<IActionResult> Index()
	{
		var indexproduct = await _appDbContext.Products.ToListAsync();
		return View("Index", indexproduct);
	}


	public async Task<IActionResult> AddProduct() => View("AddProduct");
    [HttpPost]
    public async Task<IActionResult>AddProduct(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View("Index");
        }
        var products = product.Adapt<Product>();
        if (product.Price<0 ) return View("Manfiy bo'lishi mumkin emas");
        _appDbContext.Products.Add(products);
        await _appDbContext.SaveChangesAsync();
        var products2 = await _appDbContext.Products.ToListAsync();
        return View("Index", products2);
    }

	public async Task<IActionResult> UpdateProduct() => View("UpdateProduct");
    [HttpPost]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (!ModelState.IsValid) return View("Index");
        var productfind = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        productfind.ItemName = product.ItemName;
        productfind.Price = product.Price;
        productfind.Quantiy = product.Quantiy;
        productfind.TotalPrice = product.TotalPrice;
        _appDbContext.Products.Update(productfind);
        await _appDbContext.SaveChangesAsync();

        var returnproduct = await _appDbContext.Products.ToListAsync();
        return View("Index", returnproduct);
    }

	public IActionResult DeleteProduct() => View();

	[HttpPost]
    public async Task <IActionResult> DeleteProduct(int id)
    {
        if (!ModelState.IsValid) return View("Index");
        var findProduct = await _appDbContext.Products.FirstOrDefaultAsync(d=>d.Id == id);
        _appDbContext.Products.Remove(findProduct);
        await _appDbContext.SaveChangesAsync();

        var find2 = await _appDbContext.Products.ToListAsync();
        return View("Index", find2);
    }
}
