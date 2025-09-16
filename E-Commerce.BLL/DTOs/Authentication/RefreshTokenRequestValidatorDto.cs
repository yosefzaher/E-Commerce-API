using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.DTOs.Authentication;
public class RefreshTokenRequestValidatorDto : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestValidatorDto()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
