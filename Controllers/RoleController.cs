using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels;
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
      [HttpPost]
      [Route("AddRole")]
      public async Task<IActionResult> AddRole(Role role)
      {
         await _roleService.Create(role);
         return StatusCode(200, "Успех");
      }
      [HttpPost]
      [Route("AddAdminRoleToUser")]
      public async Task<IActionResult> AddAdminRoleToUser(Guid userId)
      {
         var user = _userService.GetUserById(userId);
         if (user == null)
            return StatusCode(200, "Не Успех");
         var adminRole = await _roleService.GetRoleByName("admin");
         await _roleService.Save(userId, adminRole.Id);
         return StatusCode(200, "Успех");
      }
      [HttpPost]
      [Route("AddModerRoleToUser")]
      public async Task<IActionResult> AddModerRoleToUser(Guid userId)
      {
         var user = _userService.GetUserById(userId);
         if (user == null)
            return StatusCode(200, "Не Успех");
         var adminRole = await _roleService.GetRoleByName("moder");
         await _roleService.Save(userId, adminRole.Id);
         return StatusCode(200, "Успех");
      }
   }
}
