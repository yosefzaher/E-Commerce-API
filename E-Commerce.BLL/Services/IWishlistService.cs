using E_Commerce.BLL.DTOs.Product;
using E_Commerce.BLL.DTOs.Wishlist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services;
public interface IWishlistService
{
    Task<bool> AddToWishlistAsync(string userId, int productId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFromWishlistAsync(string userId, int productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductResponseDto>> GetUserWishlistAsync(string userId, CancellationToken cancellationToken = default);
}
