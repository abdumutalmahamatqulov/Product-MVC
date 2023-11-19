using Product_MVC.Data;
using Product_MVC.Dto_s;
using Product_MVC.Entities;
using Product_MVC.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Product_MVC.Services;
public class AuthService 
{
    private readonly AppDbContext _appDbContext;
	private readonly IUserRepository _userRepository;
	public AuthService(AppDbContext appDbContext, IUserRepository userRepository)
	{
		_appDbContext = appDbContext;
		_userRepository = userRepository;
	}

	public async Task Registor(RegistorDto registorDto)
	{
		var userList = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Email == registorDto.Email);
		if (userList != null) throw new Exception("User exelist");
		var userToRegister = registorDto.Adapt<User>();
		_appDbContext.Users.Add(userToRegister);
		//await _appDbContext.SaveChangesAsync();
	}

	public bool Login(LoginDto loginDto)
	{
		if (loginDto.Email != null && loginDto != null)
		{
			var user = _userRepository.GetByUserEmail(loginDto.Email);

			if (user.Result.PasswordHash == loginDto.Password)
			{
				return true;
			}
			return false;
		}
		return false;
	}

}