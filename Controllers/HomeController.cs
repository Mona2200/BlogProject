using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace BlogProject.Controllers
{
   public class HomeController : Controller
   {
      private readonly ILogger<HomeController> _logger;
      private readonly RoleService _roleService = new RoleService();
      private readonly UserService _userService = new UserService();

      public HomeController(ILogger<HomeController> logger)
      {
         _logger = logger;
         _logger.LogDebug(1, "NLog находится внутри HomeController");
      }

      public async Task<IActionResult> Index()
      {
         if (HttpContext.User.Identity.IsAuthenticated)
         {
            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
            var user = await _userService.GetUserByEmail(claimEmail);

            _logger.LogInformation($"Пользователь {user.Id} зашёл в приложение.");

            return RedirectToRoute(new { controller = "User", action = "Main" });
         }
         else
         {
            _logger.LogInformation($"Неизвестный пользователь зашёл в приложение.");

            return View();
         }
      }

      public IActionResult Register()
      {
         return RedirectToRoute(new { controller = "User", action = "Register" });
      }

      [HttpGet]
      [Route("Authenticate")]
      public async Task<IActionResult> Authenticate(LoginViewModel model)
      {
         var user = await _userService.GetUserByEmail(model.Email);
         if (user == null)
         {
            ModelState.AddModelError("Password", "Пользователь не найдён");
            return View("Index");
         }

         if (user.Password != model.Password)
         {
            ModelState.AddModelError("Password", "Неверный пароль");
            return View("Index");
         }

         var role = await _roleService.GetRoleByUserId(user.Id);

         var claims = new List<Claim>
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
            };

         for (int i = 0; i < role.Length; i++)
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role[i].Name));

         var claimsIdentity = new ClaimsIdentity(claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
         await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

         _logger.LogInformation($"Пользователь {user.Id} авторизовался.");

         return RedirectToRoute(new { controller = "User", action = "Main" });
      }

      [HttpGet]
      [Route("Logout")]
      public async Task<IActionResult> Logout()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);

         _logger.LogInformation($"Пользователь {user.Id} вышел из аккаунта.");

         await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         return RedirectToAction("Index", "Home");
      }

      public IActionResult Privacy()
      {
         return View();
      }
   }
}