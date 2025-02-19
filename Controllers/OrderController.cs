using LogistTrans.Context;

namespace LogistTrans.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "client")]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.Name);
        if (userIdClaim == null)
        {
            return RedirectToAction("Login", "Account"); // Перенаправление на страницу входа, если Claim не найден
        }

        var client = await _context.Clients
            .Include(c => c.Login)
            .FirstOrDefaultAsync(c => c.Login.Login == userIdClaim.Value);

        if (client == null)
        {
            return NotFound();
        }

        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.ClientId == client.Id)
            .ToListAsync();

        if (orders == null || !orders.Any())
        {
            ViewBag.Message = "У вас пока нет заказов.";
        }

        return View(orders);
    }
}

