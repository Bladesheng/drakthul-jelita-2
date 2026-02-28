using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DrakthulJelita.Web.Controllers;

public class AuthController(IConfiguration config) : Controller
{
    [HttpGet("/auth")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("/auth/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string password)
    {
        var adminPassword = config["AdminPassword"];

        if (string.IsNullOrEmpty(adminPassword) || password != adminPassword)
            return Redirect("https://www.youtube.com/watch?v=dQw4w9WgXcQ");

        var claims = new[] { new Claim(ClaimTypes.Name, "admin") };
        var identity =
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return RedirectToAction("Index", "Screenshots");
    }

    [HttpPost("/auth/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Screenshots");
    }
}