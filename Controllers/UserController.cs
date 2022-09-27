using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Controllers
{
   public class UserController : Controller
   {
      private readonly ILogger<UserController> _logger;
      private readonly IMapper _mapper;
      private readonly UserService _userService = new UserService();
      public UserController(ILogger<UserController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }
      [HttpGet]
      [Route("GetUsers")]
      public async Task<User[]> GetUsers()
      {
         var users = await _userService.GetUsers();
         return users;
      }
      [HttpGet]
      [Route("GetUserById")]
      public async Task<User> GetUserById(Guid id)
      {
         var user = await _userService.GetUserById(id);
         if (user == null)
            return null;
         return user;
      }
      [HttpPost]
      [Route("Register")]
      public async Task<IActionResult> Register(UserViewModel view)
      {
         var user = _mapper.Map<UserViewModel, User>(view);
         await _userService.Save(user);
         return StatusCode(200, "Успех");
      }
      [HttpPut]
      [Route("Edit")]
      public async Task<IActionResult> Edit(Guid id, UserViewModel view)
      {
         var editUser = await _userService.GetUserById(id);
         if (editUser == null)
            return StatusCode(200, "Не Успех");
         var newUser = _mapper.Map<UserViewModel, User>(view);
         await _userService.Update(editUser, newUser);
         return StatusCode(200, "Успех");
      }
      [HttpDelete]
      [Route("Delete")]
      public async Task<IActionResult> Delete(Guid id)
      {
         var user = await _userService.GetUserById(id);
         if (user == null)
            return StatusCode(200, "Не Успех");
         await _userService.Delete(user);
         return StatusCode(200, "Успех");
      }
   }
}
