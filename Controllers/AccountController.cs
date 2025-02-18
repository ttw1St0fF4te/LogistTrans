using LogistTrans.Context;
using LogistTrans.Models;

namespace LogistTrans.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _context.UserLogins.AnyAsync(u => u.Login == model.Login))
            {
                ModelState.AddModelError("", "Пользователь с таким логином уже существует.");
            }
            else if (await _context.Clients.AnyAsync(c => c.Email == model.Email))
            {
                ModelState.AddModelError("", "Клиент с таким email уже существует.");
            }
            else
            {
                var passHash = ComputeSha256Hash(model.Password);
                var userLogin = new UserLogin
                {
                    Login = model.Login,
                    PassHash = passHash,
                    RoleId = 4 // Роль клиента
                };

                await _context.UserLogins.AddAsync(userLogin);
                await _context.SaveChangesAsync();

                var client = new Client
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MiddleName = model.MiddleName,
                    CompanyName = model.CompanyName,
                    Phone = model.Phone,
                    Email = model.Email,
                    LoginId = userLogin.Id,
                    RegistrationDate = DateTime.UtcNow // Установка текущей даты и времени
                };

                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userLogin = await _context.UserLogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == model.Login);

            if (userLogin != null && userLogin.PassHash == ComputeSha256Hash(model.Password))
            {
                // Получаем роль пользователя
                string roleName = userLogin.Role.RoleName;

                // Создаем список Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userLogin.Login), // Логин пользователя
                    new Claim(ClaimTypes.Role, roleName),       // Роль пользователя
                    new Claim("UserId", userLogin.Id.ToString()) // ID пользователя
                };

                // Создаем ClaimsIdentity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Аутентифицируем пользователя
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true, // Сессия будет сохраняться после закрытия браузера
                        ExpiresUtc = DateTime.UtcNow.AddHours(1) // Время жизни сессии
                    });

                // Успешная аутентификация, перенаправление на главную страницу
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Неверный логин или пароль.");
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
