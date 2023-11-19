using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using Product_MVC.Data;
using Product_MVC.Entities;
using BadHttpRequestException = Microsoft.AspNetCore.Http.BadHttpRequestException;

namespace Product_MVC.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

    public async Task<User> GetByUserEmail(string email) => await _appDbContext.Users.FirstOrDefaultAsync(e => e.Email == email) ?? throw new BadHttpRequestException("User not Found");
}
