using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Product_MVC.Entities;
using Product_MVC.Repositories;

namespace Product_MVC.Controllers;
//[Authorize(Roles = "ADMIN")]
[Route("api/[controller]")]
[ApiController]
public class ProductsApiController : ControllerBase
{
	private readonly IProductRepository _productRepository;
	private readonly UserManager<User> _userManager;

	public ProductsApiController(UserManager<User> userManager, IProductRepository productRepository)
	{
		_userManager = userManager;
		_productRepository = productRepository;
	}
	[HttpGet]
	public async Task<IActionResult> Index() => Ok(await _productRepository.GetAllProducts());
	[HttpPost]
	public async Task <IActionResult >Create(Product product)
	{
		var user = await _userManager.GetUserAsync(HttpContext.User);
		await _productRepository.CreateProductAsync(product);
		await _productRepository.CreateAudit(product, null, "Create", user);
		return Ok();
	}
	[HttpPut]
	public async Task <IActionResult>Update(int id, Product product)
	{
		if (id != product.Id)
			return NotFound();
		var user = await _userManager.GetUserAsync(HttpContext.User);
		var oldProduct = await _productRepository.GetOldValueAsync(id);
		var newProduct = await _productRepository.UpdateProductAsync(product);
		await _productRepository.CreateAudit(newProduct, oldProduct, "Update", user);
		return Ok(newProduct);
	}
	[HttpDelete]
	public async Task <IActionResult>DeleteConfirmed(int id)
	{
		var product = await _productRepository.DeleteProductAsync(id);
		if(product == null) return NotFound();

		var user = await _userManager.GetUserAsync(HttpContext.User);
		await _productRepository.CreateAudit(product, null, "Delete", user);
		return Ok();
	}
}
