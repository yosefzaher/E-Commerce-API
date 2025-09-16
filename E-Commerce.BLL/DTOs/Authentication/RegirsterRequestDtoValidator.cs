using E_Commerce.BLL.Abstraction.Consts;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace E_Commerce.BLL.DTOs.Authentication;
public class RegirsterRequestDtoValidator : AbstractValidator<RegirsterRequestDto>
{
    public RegirsterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => !Regex.IsMatch(email, RegexPatterns.NoHtmlTags));

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 digits & should contains lowercase, nonalphanumeric and upper case.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100)
            .Must(firstName => !Regex.IsMatch(firstName, RegexPatterns.NoHtmlTags));

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100)
            .Must(lastName => !Regex.IsMatch(lastName, RegexPatterns.NoHtmlTags));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(RegexPatterns.PhoneNumber)
            .WithMessage("Invalid phone number, Please enter a valid Egyptian phone number");
    }
}
