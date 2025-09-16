using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Models;
public class Wishlist
{
    public int WishlistId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = default!;
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
}
