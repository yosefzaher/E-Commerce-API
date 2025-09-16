using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Abstraction.Consts;
public static class RegexPatterns
{
    public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~'|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
    public const string PhoneNumber = @"^(?:\+20|0020|0)?1[0125][0-9]{8}$";
    public const string NoHtmlTags = @"<.*?>";
}
