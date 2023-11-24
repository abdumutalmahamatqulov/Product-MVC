using FluentValidation;
using Product_MVC.Dto_s;
using Product_MVC.Funtion;

namespace Product_MVC.Volidation;

public class RegisterModelValidator: AbstractValidator<RegistorDto>
{
    public RegisterModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Must(CheckEmail.HaveCapitalLetter).WithMessage("Password must contain at least one capital letter.");

    }
}
