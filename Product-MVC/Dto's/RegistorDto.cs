using System.ComponentModel.DataAnnotations;
using Product_MVC.Entities;
using Product_MVC.Entities.Enum;

namespace Product_MVC.Dto_s;

public class RegistorDto
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Name")]
    public string UserName { get; set; }
    [Display(Name= "Email Address")]
    [Required(ErrorMessage = "Error address is required")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Email maydoni to'ldirilishi shart")]
    [DataType(DataType.Password)]
    //[CustomEmailValidation(ErrorMessage = "Faqat @gmail.com shunga ega manzillar qabul qiliniadi")]
    public string Password { get; set; }
    public ERole? Role { get; set; }
}

