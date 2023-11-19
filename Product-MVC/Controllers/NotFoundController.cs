using Microsoft.AspNetCore.Mvc;

namespace Product_MVC.Controllers;

public class NotFoundController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
