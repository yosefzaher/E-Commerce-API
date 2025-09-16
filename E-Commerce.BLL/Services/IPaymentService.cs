using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services;
public interface IPaymentService
{
    string CreatePaymentIntent(decimal amount, string currency);
}
