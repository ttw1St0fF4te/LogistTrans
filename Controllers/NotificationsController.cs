namespace LogistTrans.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LogistTrans.Context;
using LogistTrans.Models;

public class NotificationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotificationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "client")]
    public async Task<IActionResult> Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.Name);
        if (userIdClaim == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var client = await _context.Clients
            .Include(c => c.Login)
            .FirstOrDefaultAsync(c => c.Login.Login == userIdClaim.Value);

        if (client == null)
        {
            return NotFound();
        }

        var notifications = await _context.Notifications
            .Where(n => n.ClientId == client.Id)
            .OrderByDescending(n => n.SentDate)
            .ToListAsync();

        return View(notifications);
    }
}