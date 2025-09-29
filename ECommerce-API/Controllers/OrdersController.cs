using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce_API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService order) : ControllerBase
{
    private readonly IOrderService _order = order;

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("PlaceOrder/{Id}")]
    public async Task<IActionResult> PlaceOrder(string Id, CancellationToken cancellationToken = default)
    {
        bool isPlaced = await _order.CheckoutAsync(Id, cancellationToken);
       
        return (isPlaced) ? NoContent() : BadRequest();
    }

    [HttpGet("GetProductsDetails/{Id}")]
    public async Task<IActionResult> GetOrderProductDetails(string Id, int orderId, CancellationToken cancellationToken = default)
    {
        var orderProducts = await _order.GetOrderProductsDetailsAsync(Id, orderId, cancellationToken);

        return orderProducts.Any() ? Ok(orderProducts) : BadRequest($"There is no Products in this order {orderId}");
    }

    [HttpGet("GetUserOrders/{Id}")]
    public async Task<IActionResult> GetUserOrders(string Id, CancellationToken cancellationToken = default)
    {
        var userOrders = await _order.GetUserOrdersAsync(Id, cancellationToken);

        return userOrders.Any() ? Ok(userOrders) : BadRequest();
    }

    [HttpDelete("DeleteUserOrder/{Id}")]
    public async Task<IActionResult> DeleteUserOrder(string Id, int orderId, CancellationToken cancellationToken = default)
    {
        var userOrders = await _order.RemoveUserOrderAsync(Id, orderId, cancellationToken);

        return userOrders ? NoContent() : BadRequest();
    }

    [HttpDelete("ClearUserOrders/{Id}")]
    public async Task<IActionResult> ClearUserOrders(string Id,CancellationToken cancellationToken = default)
    {
        var userOrders = await _order.RemoveUserAllOrdersAsync(Id, cancellationToken);

        return userOrders ? NoContent() : BadRequest();
    }

    [HttpPatch("{orderId}/mark-shipped")]
    public async Task<IActionResult> MarkShipped(int orderId, string userId, CancellationToken cancellationToken)
    {
        var result = await _order.MarkOrderAsShipped(orderId, userId, cancellationToken);

        if (result == 0)
            return NotFound(); 

        return Ok(new {OrderId = result}); 
    }

    [HttpGet("GetUserShippedOrders/{userId}")]
    public async Task<IActionResult> GetUserShippedOrders(string userId, CancellationToken cancellationToken = default)
    {
        var userOrders = await _order.GetUserShippedOrdersAsync(userId, cancellationToken);

        return userOrders.Any() ? Ok(userOrders) : BadRequest(new {Message = "There is no Shipped Orders"});
    }

    [HttpGet("GetAllShippedOrders")]
    public async Task<IActionResult> GetAllShippedOrders(CancellationToken cancellationToken = default)
    {
        var shippedOrders = await _order.GetAllShippedOrdersAsync(cancellationToken);

        return shippedOrders.Any() ? Ok(shippedOrders) : BadRequest(new { Message = "There is no shipped orders"});

    }

    [HttpDelete("ClearUserShippedOrders/{userId}")]
    public async Task<IActionResult> ClearUserShippedOrders(string userId, int orderId, CancellationToken cancellationToken = default)
    {
        var userShippedOrders = await _order.RemoveUserShippedOrdersAsync(userId, orderId, cancellationToken);

        return userShippedOrders ? NoContent() : BadRequest(new {Message = "There is no Shipped Orders"});
    }

}
