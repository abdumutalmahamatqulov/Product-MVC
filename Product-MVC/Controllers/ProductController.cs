using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Product_MVC.Data;
using Product_MVC.Dto_s;
using Product_MVC.Entities;
using Product_MVC.Entities.Enum;
using Product_MVC.Repositories;

namespace Product_MVC.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<User> _userManager;
    private readonly IProductRepository _productRepository;
	private readonly IToastNotification _notification;
    public ProductController(AppDbContext appDbContext, UserManager<User> userManager, IProductRepository productRepository, IToastNotification notification)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _productRepository = productRepository;
        _notification = notification;
    }



	public async Task<IActionResult> Index()
	{
		var indexproduct = await _appDbContext.Products.ToListAsync();
		foreach (var product in indexproduct)
		{
			product.TotalPrice = (product.Price * product.Quantiy) * (1 + 0.1);
			product.TotalPrice = Math.Floor(product.TotalPrice);
			await _appDbContext.SaveChangesAsync();
		}
		if (User.IsInRole("ADMIN"))
		{
		_notification.AddInfoToastMessage("ADMIN");
		}
		else
		{
			_notification.AddInfoToastMessage("USER");
		}
		return View(indexproduct);
	}


	public async Task<IActionResult> AddProduct() => View();
    [HttpPost]
    public async Task<IActionResult>AddProduct([Bind("Id,ItemName,Quantiy,Price")]Product product)
    {
        if(product.Quantiy<0 && product.Price < 0)
		{
			_notification.AddErrorToastMessage("Manfiy kiritish mumkin emas");
			return View(product);
		}
		if (!ModelState.IsValid)
		{

			_notification.AddErrorToastMessage("Hatolik bor");
			return View(product);
		}
		double totalPrice = (product.Price * product.Quantiy) * (1 + 0.1);
		product.TotalPrice = totalPrice;

		var user = await _userManager.GetUserAsync(HttpContext.User);
        await _productRepository.CreateProductAsync(product);
		await _productRepository.CreateAudit(product, null, "AddProduct", user);
	
		return View("Index", await _appDbContext.Products.ToListAsync());
    }

	public async Task<IActionResult> UpdateProduct(int id)
	{
		try
		{
			if (id == null) return NotFound();

			var product = await _productRepository.GetProductByIdAsync(id);
			if (product == null) return NotFound();

			return View(product);
		}
		catch (Exception ex)
		{
			return RedirectToAction("Index", "UpdateProduct");
		}
	}
	[HttpPost]
    public async Task<IActionResult> UpdateProduct(int id,[Bind("Id,ItemName,Quantiy,Price")] Product product)
    {
		if (id != product.Id) return NotFound();
		if (!ModelState.IsValid) return View(product);
		try
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			var oldProduct = await _productRepository.GetOldValueAsync(product.Id);
			var newProduct = await _productRepository.UpdateProductAsync(product);
			await _productRepository.CreateAudit(newProduct, oldProduct, "UpdateProduct", user);
		}
		catch (DbUpdateConcurrencyException)
		{
			if (_productRepository.GetProductByIdAsync(product.Id) == null)
				return NotFound();
			else
				throw;
		}
        var allresult = await _appDbContext.Products.ToListAsync();
        foreach(var produc in allresult)
        {
			produc.TotalPrice = (produc.Price * produc.Quantiy) * (1 + 0.1);
			produc.TotalPrice = Math.Floor(produc.TotalPrice);
			await _appDbContext.SaveChangesAsync();
		}
        return View("Index", allresult);
	}
	[Authorize(Roles = "ADMIN")]
	public async Task<IActionResult> Details(int id)
	{
		try
		{
			if (id == null) return NotFound();

			var product = await _productRepository.GetProductByIdAsync(id);
			if (product == null) return NotFound();

			return View(product);
		}
		catch (Exception ex)
		{
			return RedirectToAction("Index", "NotFoundPage");
		}

	}
	public IActionResult DeleteProduct() => View();

	[HttpPost]
    public async Task <IActionResult> DeleteProduct(int id)
    {

        if (!ModelState.IsValid)
			
			return View("Index");
        var findProduct = await _appDbContext.Products.FirstOrDefaultAsync(d=>d.Id == id);
        if(findProduct != null)
        {
            _appDbContext.Products.Remove(findProduct);
            await _appDbContext.SaveChangesAsync();
        }

        var find2 = await _appDbContext.Products.ToListAsync();
        return View("Index", find2);
    }
}

