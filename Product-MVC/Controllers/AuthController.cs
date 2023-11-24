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
using NToastNotify;
using Product_MVC.Funtion;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using FluentValidation.Validators;

namespace Product_MVC.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
	private readonly SignInManager<User> _signInManager;
	private readonly IToastNotification _notification;
	public AuthController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IToastNotification notification, SignInManager<User> signInManager)
	{
		_roleManager = roleManager;
		_userManager = userManager;
		_notification = notification;
		_signInManager = signInManager;
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
			_notification.AddWarningToastMessage("Please Enter Email Or Password");
			return View(loginDto);
		}
		var login = await _userManager.FindByEmailAsync(loginDto.Email);
		if (login != null)
		{
			var isPasswordValid = await _userManager.CheckPasswordAsync(login, loginDto.Password);


			if (isPasswordValid)
			{
				var result = await _signInManager.PasswordSignInAsync(login, loginDto.Password,false,false);
				if (result.Succeeded)
				{
					var rol = await _userManager.GetRolesAsync(login);
					foreach(var rolle in rol)
					{
						if (rolle is not null)
						{
							return RedirectToAction("Index", "Product");
						}
						
					}
				}

			}
			TempData["Error"] = "Parol noto'g'ri";
			return View(loginDto);
		}
		_notification.AddErrorToastMessage("Error Login");
		TempData["Error"] = "Foydalanuvchi topilmadi";
		return RedirectToAction("Login", "Auth");
	}
	public async Task<IActionResult> Registor()
	{
		var response = new RegistorDto();
		return View(response);
	}


	[HttpPost]

	public async Task<IActionResult> RegistorConiform(RegistorDto registerDto)
	{
		if (!ModelState.IsValid)
			return View(registerDto);

		if (!CheckEmail.IsValidEmail(registerDto.Email))
		{
			_notification.AddErrorToastMessage("Emailda yoki Passwordda hatolik bor");
			return View("Registor");
		}

			var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("Email", "Elektron pochta allaqachon ishlatilgan.");
				return View(registerDto);
			}

			var newUser = new User()
			{
				Email = registerDto.Email,
				UserName = registerDto.UserName,
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
					return RedirectToAction("Login", "Auth");
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
