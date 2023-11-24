using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using Product_MVC.Data;
using Product_MVC.Dto_s;
using Product_MVC.Entities;
using Product_MVC.Funtion;
using BadHttpRequestException = Microsoft.AspNetCore.Http.BadHttpRequestException;

namespace Product_MVC.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;
	private readonly UserManager<User> _userManager;

    public UserRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;
	public async Task<RegistorDto> Register(RegistorDto model)
	{
		if (!CheckEmail.IsValidEmail(model.Email))
			throw new Exception("Invalid email address format");
		var existUser = await _userManager.FindByEmailAsync(model.Email);
		if (existUser != null)
			throw new Exception("Email already taken ");
		var user = new User { UserName = model.UserName, Email = model.Email };
		var result = await _userManager.CreateAsync(user, model.Password);
		if (result.Succeeded)
		{
			await _userManager.AddToRoleAsync(user, "USER");
			await _appDbContext.SaveChangesAsync();
		}
		foreach (var error in result.Errors)
			throw new Exception($"{error.Description}");

		return model ?? new RegistorDto();

	}
	public async Task<User> GetByUserEmail(string email) => await _appDbContext.Users.FirstOrDefaultAsync(e => e.Email == email) ?? throw new BadHttpRequestException("User not Found");
}
