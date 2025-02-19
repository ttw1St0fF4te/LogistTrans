using LogistTrans.Context;
using LogistTrans.Models;
using Microsoft.AspNetCore.Authorization;

namespace LogistTrans.Controllers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }

    [Authorize(Roles = "logist")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "logist"), HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(product);
    }

    [Authorize(Roles = "logist")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [Authorize(Roles = "logist"), HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "client")]
    [HttpPost]
    public async Task<IActionResult> Order(int id, int quantity)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        if (quantity <= 0 || product.Quantity < quantity)
        {
            ModelState.AddModelError(string.Empty, "Недостаточно товара на складе или некорректное количество.");
            return View(product);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.Name);
        if (userIdClaim == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect to login page if Claim is not found
        }

        var client = await _context.Clients
            .Include(c => c.Login)
            .FirstOrDefaultAsync(c => c.Login.Login == userIdClaim.Value);

        if (client == null)
        {
            return NotFound();
        }

        var order = new Order
        {
            ClientId = client.Id,
            OrderDate = DateTime.UtcNow,
            Status = "Заказано",
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = quantity
                }
            }
        };

        // Logic for calculating the delivery date
        var distance = new Random().Next(100, 1000); // Random distance from 100 to 1000 km
        var speed = new Random().Next(50, 200); // Random speed from 50 to 200 km/h
        var deliveryTime = TimeSpan.FromHours(distance / (double)speed);
        order.DeliveryDate = order.OrderDate.Add(deliveryTime);

        // Decrease the product quantity
        product.Quantity -= quantity;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Orders"); // Redirect to the orders page
    }
}
