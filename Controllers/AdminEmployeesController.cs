using Microsoft.AspNetCore.Mvc.Rendering;

namespace LogistTrans.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using LogistTrans.Context;
using LogistTrans.Models;

[Authorize(Roles = "admin")]
public class AdminEmployeesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminEmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees
            .Include(e => e.Login)
            .ToListAsync();

        return View(employees);
    }

    public IActionResult Create()
    {
        ViewBag.LoginId = new SelectList(_context.UserLogins, "Id", "Login");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        ViewBag.LoginId = new SelectList(_context.UserLogins, "Id", "Login", employee.LoginId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}