using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.DTOs.Order;
public class UserOrderResponseDto
{
    public int OrderId { get; set; }  
    public string UserId { get; set; } = string.Empty; 
    public int CountOfItems { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public decimal TotalAmoutForeachOrder { get; set; }
}
