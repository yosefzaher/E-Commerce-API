using AutoMapper;
using E_Commerce.BLL.DTOs.Authentication;
using E_Commerce.BLL.DTOs.User;
using E_Commerce.DAL.Models;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Mapping;
public class RegisterUserMap : Profile
{
    public RegisterUserMap()
    {
        CreateMap<RegirsterRequestDto, ApplicationUser>()
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
           .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

    }
}
