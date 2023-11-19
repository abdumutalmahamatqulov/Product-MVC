using Product_MVC.Entities;

namespace Product_MVC.Repositories
{
    public interface IUserRepository
    {
        Task<User>GetByUserEmail(string email);
    }
}
