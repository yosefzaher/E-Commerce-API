using E_Commerce.BLL.Abstraction.Consts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_Commerce.BLL.DTOs.Authentication;
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => !Regex.IsMatch(email, RegexPatterns.NoHtmlTags));

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
