using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Controllers
{
   public class RoleController : Controller
   {
      private readonly ILogger<UserController> _logger;
      private readonly IMapper _mapper;
      private readonly RoleService _roleService = new RoleService();
      private readonly UserService _userService = new UserService();

      public RoleController(ILogger<UserController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }

      [Authorize(Roles = "admin")]
      [HttpPost]
      [Route("AddRole")]
      public async Task<IActionResult> AddRole(Role role)
      {
         await _roleService.Create(role);
         return Ok();
      }

      [Authorize(Roles = "admin")]
      [HttpGet]
      [Route("AddAdminRoleToUser")]
      public async Task<IActionResult> AddAdminRoleToUser(Guid userId)
      {
         var user = await _userService.GetUserById(userId);
         if (user == null)
            return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });

         var roles = await _roleService.GetRoleByUserId(userId);
         if (roles.FirstOrDefault(r => r.Name == "admin") == null)
         {
            var adminRole = await _roleService.GetRoleByName("admin");
            await _roleService.Save(userId, adminRole.Id);
         }

         return RedirectToAction("GetAllUsers", "User");
      }

      [Authorize(Roles = "admin")]
      [HttpGet]
      [Route("AddModerRoleToUser")]
      public async Task<IActionResult> AddModerRoleToUser(Guid userId)
      {
         var user = _userService.GetUserById(userId);
         if (user == null)
            return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });

         var roles = await _roleService.GetRoleByUserId(userId);
         if (roles.FirstOrDefault(r => r.Name == "moder") == null)
         {
            var moderRole = await _roleService.GetRoleByName("moder");
            await _roleService.Save(userId, moderRole.Id);
         }

         return RedirectToAction("GetAllUsers", "User");
      }

      [Authorize(Roles = "admin")]
      [HttpGet]
      [Route("GetRoleAdmin")]
      public async Task<Role> GetRoleAdmin()
      {
         var role = await _roleService.GetRoleByName("admin");
         return role;
      }
   }
}
