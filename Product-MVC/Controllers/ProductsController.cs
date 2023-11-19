using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product_MVC.Entities;
using Product_MVC.Repositories;

namespace Product_MVC.Controllers;

public class ProductsController : Controller
{
	private readonly IProductRepository _productRepository;
	private readonly UserManager<User> _userManager;
	public ProductsController(UserManager<User> userManager, IProductRepository productRepository)
	{
		_userManager = userManager;
		_productRepository = productRepository;
	}
	[Authorize(Roles = "AMIN,USER")]
	public async Task<IActionResult> Index()
	{
		var products = await _productRepository.GetAllProducts();
		var productview = products.Select(p=>new Product
		{
			Id = p.Id,
			Price = p.Price,
			Quantiy = p.Quantiy,
			TotalPrice = p.TotalPrice
		}).ToList();
		return View(productview);
	}
	[Authorize(Roles = "ADMIN")]
	public IActionResult Create() => View();
	[Authorize(Roles = "ADMIN")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task <IActionResult>Create([Bind("Id,Price,Quantiy,TotalPrice,Email")]Product product)
	{
		if(!ModelState.IsValid) return View (product);

		var user = await _userManager.GetUserAsync(User);
		await _productRepository.CreateProductAsync(product);
		return RedirectToAction(nameof(Index));
	}
	[Authorize(Roles = "ADMIN")]
	public async Task<IActionResult> Update(int id)
	{
		try
		{
			if (id == null) return NotFound();

			var product = await _productRepository.GetProductByIdAsync(id);
			if (product == null) return NotFound();

			return View(product);
		}
		catch (Exception)
		{
			return RedirectToAction("Index", "Home");
		}
	}
	[Authorize(Roles = "ADMIN")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Update(int id, [Bind("Id,Price,Quantiy,TotalPrice")] Product product)
	{
		if (id != product.Id)
			return NotFound();
		if (!ModelState.IsValid) return View();

		try
		{
			var user = await _userManager.GetUserAsync(HttpContext.User);
			var oldProduct = await _productRepository.GetOldValueAsync(id);
			var newProduct = await _productRepository.UpdateProductAsync(product);
			await _productRepository.CreateAudit(newProduct, oldProduct, "Edit", user);
		}
		catch (DbUpdateConcurrencyException)
		{
			if (_productRepository.GetProductByIdAsync(product.Id) == null)
				return NotFound();
			else
				throw;
		}
		return RedirectToAction(nameof(Index));
	}
	[Authorize(Roles = "ADMIN")]
	public async Task<IActionResult>Delete(int id)
	{
		try
		{
			if (id == null) return NotFound();
			var product = await _productRepository.GetProductByIdAsync(id);
			if(product == null) return NotFound();
			return View(product);
		}
		catch
		{
			return RedirectToAction("Index", "Home");
		}
	}
	[Authorize(Roles = "ADMIN")]
	[HttpPost,ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task <IActionResult>DeleteCofirmed(int id)
	{
		var product = await _productRepository.DeleteProductAsync(id); if(product == null) return NotFound();
		return RedirectToAction(nameof(Index));
	}
}
