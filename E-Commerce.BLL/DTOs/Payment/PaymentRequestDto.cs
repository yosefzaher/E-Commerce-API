using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.DTOs.Payment;
public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "egp";
}
