using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WishlistsController(IWishlistService wishlistService) : ControllerBase
{
    private readonly IWishlistService _wishlistService = wishlistService;

    [HttpGet("GetUserWishlist/{useId}")]
    public async Task<IActionResult> GetUserWishlist(string useId, CancellationToken cancellationToken)
    {
        var result = await _wishlistService.GetUserWishlistAsync(useId, cancellationToken);
        return Ok(result);
    }

    [HttpPost("AddToUserWishlist/{useId}/{productId}")]
    public async Task<IActionResult> AddToWishlist(string useId, int productId, CancellationToken cancellationToken)
    {
        var isAdded = await _wishlistService.AddToWishlistAsync(useId, productId, cancellationToken);

        return isAdded ? Ok(new { message = "Product added to wishlist successfully" }) : BadRequest();
    }

    [HttpDelete("RemoveFromUserWishlist/{useId}/{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(string useId, int productId, CancellationToken cancellationToken)
    {
        var isRemoved = await _wishlistService.RemoveFromWishlistAsync(useId, productId, cancellationToken);

        return isRemoved ? Ok(new { message = "Product removed from wishlist successfully" }) : BadRequest();
    }

}
