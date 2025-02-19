using Microsoft.AspNetCore.Mvc.Rendering;

namespace LogistTrans.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using LogistTrans.Context;
using LogistTrans.Models;


[Authorize(Roles = "admin")]
public class AdminNotificationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminNotificationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var notifications = await _context.Notifications
            .Include(n => n.Client)
            .Include(n => n.Order)
            .OrderByDescending(n => n.SentDate)
            .ToListAsync();

        return View(notifications);
    }

    public IActionResult Create()
    {
        ViewBag.ClientId = new SelectList(_context.Clients, "Id", "LastName");
        ViewBag.OrderId = new SelectList(_context.Orders, "Id", "Id");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Notification notification)
    {
        notification.SentDate = DateTime.UtcNow;
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return NotFound();
        }

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}