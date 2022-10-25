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
      }

      public IActionResult Index()
      {
         if (HttpContext.User.Identity.IsAuthenticated)
         {
            return RedirectToRoute(new { controller = "User", action = "Main" });
         }
         else
         {
            return View();
         }
      }

      public IActionResult Register()
      {
         return View();
      }

      [HttpGet]
      [Route("Authenticate")]
      public async Task<IActionResult> Authenticate(LoginViewModel model)
      {
         if (String.IsNullOrEmpty(model.Email) || String.IsNullOrEmpty(model.Password))
            return View("Index");
         var user = await _userService.GetUserByEmail(model.Email);
         if (user == null)
         {
            ModelState.AddModelError("Password", "Пользователь не найдён");
            return View("Index");
         }

         if (user.Password != model.Password)
         {
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

         return RedirectToRoute(new { controller = "User", action = "Main" });
      }

      [HttpGet]
      [Route("Logout")]
      public async Task<IActionResult> Logout()
      {
         await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         return RedirectToAction("Index", "Home");
      }

      public IActionResult Privacy()
      {
         return View();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}