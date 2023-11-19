using Product_MVC.Entities;
using Product_MVC.Entities.Enum;

namespace Product_MVC.Dto_s;

public class UserDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public ERole Role { get; set; }    
}
