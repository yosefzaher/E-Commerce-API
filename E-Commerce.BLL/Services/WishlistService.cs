using AutoMapper;
using E_Commerce.BLL.DTOs.Product;
using E_Commerce.BLL.DTOs.Wishlist;
using E_Commerce.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services;
public class WishlistService(AppDbContext context, IMapper mapper) : IWishlistService
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> AddToWishlistAsync(string userId, int productId, CancellationToken cancellationToken = default)
    {
        if(!await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken))
            return false;

        if (!await _context.Products.AnyAsync(u => u.ProductId == productId, cancellationToken))
            return false;

        var wishlist = await _context.Wishlists
            .Include(w => w.WishlistItems)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

        if(wishlist is null)
        {
            wishlist = new Wishlist
            {
                UserId = userId,
                WishlistItems = new List<WishlistItem>()
            };

            _context.Wishlists.Add(wishlist);
        }

        var isItemAlreadyInWishlist = wishlist.WishlistItems.Any(wi => wi.ProductId == productId);

        if(!isItemAlreadyInWishlist)
        {
            wishlist.WishlistItems.Add(new WishlistItem
            {
                ProductId = productId,
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> RemoveFromWishlistAsync(string userId, int productId, CancellationToken cancellationToken = default)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken))
            return false;

        if (!await _context.Products.AnyAsync(u => u.ProductId == productId, cancellationToken))
            return false;

        var wishlist = await _context.Wishlists
            .Include(w => w.WishlistItems)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

        if (wishlist is null)
            return false;

        var item = wishlist.WishlistItems.FirstOrDefault(wi => wi.ProductId == productId);

        if (item is null) 
            return false;

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;

    }
    public async Task<IEnumerable<ProductResponseDto>> GetUserWishlistAsync(string userId, CancellationToken cancellationToken = default)
    {
        var wishlist = await _context.Wishlists
            .AsNoTracking()
            .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Product)
                .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

        if (wishlist is null)
            return Enumerable.Empty<ProductResponseDto>();

        var product = wishlist.WishlistItems.Select(wi => wi.Product);

        return _mapper.Map<IEnumerable<ProductResponseDto>>(product);


        //var items = wishlist.WishlistItems.Select(wi => new WishlistResponseDto
        //{
        //    ProductId = wi.ProductId,
        //    Title = wi.Product.Title,
        //    Price = wi.Product.Price,   
        //    Image = wi.Product.Image is not null ? Convert.ToBase64String(wi.Product.Image) : string.Empty,
        //}).ToList();

        //return items;
    }
    
}
