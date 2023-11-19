using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Product_MVC.Dto_s;
using Product_MVC.Entities;
using Product_MVC.Services;
using Product_MVC.Data;
using System.Globalization;
using System.Data;
using Product_MVC.Entities.Enum;
using Microsoft.AspNetCore.Authorization;
using Product_MVC.Repositories;

namespace Product_MVC.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
	public AuthController( RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
	{
		_roleManager = roleManager;
		_userManager = userManager;
	}

	public IActionResult Login()
    {
		var response = new LoginDto();
		return View(response);
	}

	[HttpPost]

	public async Task<IActionResult> Login(LoginDto loginDto)
	{
		if (!ModelState.IsValid)
		{
			return View(loginDto);
		}
		var login = await _userManager.FindByEmailAsync(loginDto.Email);
		if (login != null)
		{
			var isPasswordValid = await _userManager.CheckPasswordAsync(login, loginDto.Password);


			if (isPasswordValid && !await _userManager.IsInRoleAsync(login, "Admin"))
			{
				return RedirectToAction("table", "Product");
			}
			else
			{
				return RedirectToAction("Index", "Product");
			}
		}
		return RedirectToAction("Login", "Auth");
	}
	public IActionResult Registor()
	{
		var response = new RegistorDto();
		return View(response);
	}


	[HttpPost]

	public async Task<IActionResult> RegistorConiform(RegistorDto registerDto)
	{
		if (!ModelState.IsValid)
			return View(registerDto);

		var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
		if (existingUser != null)
		{
			ModelState.AddModelError("Email", "Elektron pochta allaqachon ishlatilgan.");
			return View(registerDto);
		}

		var newUser = new User()
		{
			Email = registerDto.Email,
			UserName = registerDto.Email,
		};

		var result = await _userManager.CreateAsync(newUser, registerDto.Password);
	
		if (result.Succeeded)
		{
			if (registerDto.Role == 0)
			{
				await _userManager.AddToRoleAsync(newUser, ERole.Admin.ToString());
				return RedirectToAction("Login", "Auth");
			}
			else
			{
				await _userManager.AddToRoleAsync(newUser, ERole.User.ToString());
				return RedirectToAction("table", "Product");
			}
		}
		else
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(registerDto);
		}
	}
}
