using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LogistTrans.Context;
using LogistTrans.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Route = LogistTrans.Models.Route;

namespace LogistTrans.Controllers;

public class RouteController : Controller
{
    private readonly ApplicationDbContext _context;

    public RouteController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Routes
    public async Task<IActionResult> Index()
    {
        var routes = await _context.Routes
            .Include(r => r.Order)
            .Include(r => r.Employee)
            .ToListAsync();

        return View(routes);
    }

    // GET: Routes/Create
    public async Task<IActionResult> Create()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || roleClaim.Value != "logist")
        {
            return Forbid();
        }

        ViewData["OrderId"] = new SelectList(await _context.Orders.ToListAsync(), "Id", "Id");
        ViewData["EmployeeId"] = new SelectList(await _context.Employees
            .Include(e => e.Login)
            .ThenInclude(l => l.Role)
            .Where(e => e.Login.Role.RoleName == "driver")
            .ToListAsync(), "Id", "LastName");

        return View();
    }

    // POST: Routes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Route route)
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || roleClaim.Value != "logist")
        {
            return Forbid();
        }

        var selectedOrder = await _context.Orders.FindAsync(route.OrderId);
        if (selectedOrder == null)
        {
            ModelState.AddModelError(string.Empty, "Выбранный заказ не найден.");
            ViewData["OrderId"] = new SelectList(await _context.Orders.ToListAsync(), "Id", "Id", route.OrderId);
            ViewData["EmployeeId"] = new SelectList(await _context.Employees
                .Include(e => e.Login)
                .ThenInclude(l => l.Role)
                .Where(e => e.Login.Role.RoleName == "driver")
                .ToListAsync(), "Id", "LastName", route.EmployeeId);
            return View(route);
        }

        // Populate route fields from the selected order
        route.Distance = new Random().Next(100, 1000); // Example: Random distance from 100 to 1000 km
        route.DepartureTime = selectedOrder.OrderDate;
        route.ArrivalTime = selectedOrder.DeliveryDate;
        route.TravelTime = route.ArrivalTime - route.DepartureTime;

        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        ViewData["OrderId"] = new SelectList(await _context.Orders.ToListAsync(), "Id", "Id", route.OrderId);
        ViewData["EmployeeId"] = new SelectList(await _context.Employees
            .Include(e => e.Login)
            .ThenInclude(l => l.Role)
            .Where(e => e.Login.Role.RoleName == "driver")
            .ToListAsync(), "Id", "LastName", route.EmployeeId);

        return RedirectToAction("Index");
    }

    // POST: Routes/Delete/5
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null || roleClaim.Value != "logist")
        {
            return Forbid();
        }

        var route = await _context.Routes.FindAsync(id);
        if (route == null)
        {
            return NotFound();
        }

        _context.Routes.Remove(route);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private bool RouteExists(int id)
    {
        return _context.Routes.Any(e => e.Id == id);
    }
}