using Product_MVC.Dto_s;
using Product_MVC.Entities;

namespace Product_MVC.Repositories
{
    public interface IUserRepository
    {
        Task<User>GetByUserEmail(string email);
		Task<RegistorDto> Register(RegistorDto model);

	}
}
