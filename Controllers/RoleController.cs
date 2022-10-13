using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels;
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
      [HttpPost]
      [Route("AddAdminRoleToUser")]
      public async Task<IActionResult> AddAdminRoleToUser(Guid userId)
      {
         var user = _userService.GetUserById(userId);
         if (user == null)
            return BadRequest();

         var adminRole = await _roleService.GetRoleByName("admin");
         await _roleService.Save(userId, adminRole.Id);

         var moderRole = await _roleService.GetRoleByName("moder");
         await _roleService.Save(userId, moderRole.Id);

         return Ok();
      }
      [Authorize(Roles = "admin")]
      [HttpPost]
      [Route("AddModerRoleToUser")]
      public async Task<IActionResult> AddModerRoleToUser(Guid userId)
      {
         var user = _userService.GetUserById(userId);
         if (user == null)
            return BadRequest();
         var moderRole = await _roleService.GetRoleByName("moder");
         await _roleService.Save(userId, moderRole.Id);
         return Ok();
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
