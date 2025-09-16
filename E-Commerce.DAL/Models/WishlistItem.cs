using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Models;
public class WishlistItem
{
    public int WishlistItemId { get; set; }
    public int WishlistId { get; set; }
    public int ProductId { get; set; }

    public Wishlist Wishlist { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
