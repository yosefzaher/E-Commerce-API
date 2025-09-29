using AutoMapper;
using E_Commerce.BLL.Abstraction;
using E_Commerce.BLL.DTOs.Order;
using E_Commerce.BLL.DTOs.Orders;
using E_Commerce.BLL.DTOs.Product;
using E_Commerce.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services;

public class OrderService(AppDbContext context) : IOrderService
{
    private readonly AppDbContext _context = context;
    private readonly string _orderStatus = "Pending";

    // Place to Order in Front
    public async Task<bool> CheckoutAsync(string userId, CancellationToken cancellationToken)
    {
        var cart = await _context.ShoppingCarts
            .Include(s => s.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if(cart is null || !cart.CartItems.Any())
            return false;

        var totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

        var newOrder = new Order 
        { 
            UserId = userId,
            Status = _orderStatus,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,

            OrderItems = cart.CartItems.Select(ci => new OrderItem
            { 
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Product.Price
            }).ToList()
        };

        await _context.Orders.AddAsync(newOrder);

        _context.CartItems.RemoveRange(cart.CartItems);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    // Details button in Order Page
    public async Task<IEnumerable<OrderProductDetailsResponseDto>> GetOrderProductsDetailsAsync(string userId, int orderId, CancellationToken cancellationToken)
    {
        var orderProductsDetails = await _context.Orders
            .Where(o => o.UserId == userId && o.OrderId == orderId)
            .SelectMany(o => o.OrderItems.Select(oi => new OrderProductDetailsResponseDto 
            {
                ProductTitle = oi.Product.Title,
                ProductImage = oi.Product.Image != null? Convert.ToBase64String(oi.Product.Image) : string.Empty,
                ProductPrice = oi.Product.Price,

                ProductQuantityInOrderItems = oi.Quantity,

                TotalAmountForeachProduct = oi.Product.Price * oi.Quantity
            }))
            .ToListAsync(); 

        return orderProductsDetails;
    }

    // Return Order Status = Pinding
    public async Task<IEnumerable<UserOrderResponseDto>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken)
    {
        var userOrders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Select(o => new UserOrderResponseDto
            {
                OrderId = o.OrderId,
                CountOfItems = o.OrderItems.Count(),
                OrderStatus = o.Status,
                TotalAmoutForeachOrder = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
            })
            .ToListAsync(cancellationToken);

        return userOrders;
    }

    public async Task<bool> RemoveUserOrderAsync(string userId, int orderId, CancellationToken cancellationToken)
    {
        var userOrder = await _context.Orders
            .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderId == orderId, cancellationToken);

        if (userOrder is null)
            return false;

        var existingItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
        foreach (var item in existingItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
                _context.Products.Update(product); // force tracking
            }
        }
        await _context.SaveChangesAsync(cancellationToken);

        _context.OrderItems.RemoveRange(existingItems);
        _context.Orders.Remove(userOrder);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveUserAllOrdersAsync(string userId, CancellationToken cancellationToken)
    {
        var userOrders = await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems) 
            .ToListAsync(cancellationToken);

        if (!userOrders.Any())
            return false;

        foreach (var order in userOrders)
        {
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == item.ProductId, cancellationToken);

                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _context.Products.Update(product);
                }
            }

            _context.OrderItems.RemoveRange(order.OrderItems);
        }

        _context.Orders.RemoveRange(userOrders);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<int> MarkOrderAsShipped(int orderId, string userId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId, cancellationToken);

        if (order is null)
            return 0;

        order.Status = OrderStatus.Shipped;
        await _context.SaveChangesAsync(cancellationToken);

        return order.OrderId;
    }

    public async Task<IEnumerable<UserOrderResponseDto>> GetUserShippedOrdersAsync(string userId, CancellationToken cancellationToken)
    {
        var userOrders = await _context.Orders
            .Where(o => o.UserId == userId && o.Status == OrderStatus.Shipped)
            .Select(o => new UserOrderResponseDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                CountOfItems = o.OrderItems.Count(),
                OrderStatus = o.Status,
                TotalAmoutForeachOrder = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
            })
            .ToListAsync(cancellationToken);

        return userOrders;
    }

    public async Task<IEnumerable<UserOrderResponseDto>> GetAllShippedOrdersAsync(CancellationToken cancellationToken)
    {
        var userOrders = await _context.Orders
            .Where(o => o.Status == OrderStatus.Shipped)
            .Select(o => new UserOrderResponseDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                CountOfItems = o.OrderItems.Count(),
                OrderStatus = o.Status,
                TotalAmoutForeachOrder = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
            })
            .ToListAsync(cancellationToken);

        return userOrders;
    }

    public async Task<bool> RemoveUserShippedOrdersAsync(string userId, int orderId, CancellationToken cancellationToken)
    {
        var shippedOrders = await _context.Orders
            .Where(o => o.UserId == userId && o.OrderId == orderId && o.Status == OrderStatus.Shipped)
            .ToListAsync(cancellationToken);

        if (!shippedOrders.Any())
            return false;

        _context.RemoveRange(shippedOrders);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

}