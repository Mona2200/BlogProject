using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace BlogProject.Controllers
{
    public class UserController : Controller
   {
      private readonly ILogger<UserController> _logger;
      private readonly IMapper _mapper;
      private readonly UserService _userService = new UserService();
      private readonly PostService _postService = new PostService();
      private readonly TagService _tagService = new TagService();
      private readonly CommentService _commentService = new CommentService();
      private readonly RoleService _roleService = new RoleService();
      public UserController(ILogger<UserController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      public async Task<IActionResult> Main()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);
         var userViewModel = await GetUserById(user.Id);
         return View("Main", userViewModel);
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetUsers")]
      public async Task<User[]> GetUsers()
      {
         var users = await _userService.GetUsers();
         return users;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetUserById")]
      public async Task<UserViewModel> GetUserById(Guid id)
      {
         var user = await _userService.GetUserById(id);
         if (user == null)
            return null;
         var userViewModel = _mapper.Map<User, UserViewModel>(user);
         var posts = await _postService.GetPostByUserId(user.Id);
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);
            postViewModels[i].Tags = await _tagService.GetTagByPostId(post.Id);
            var comments = await _commentService.GetCommentByPostId(post.Id);
            var commentsViewModels = new CommentViewModel[comments.Length];
            var j = 0;
            foreach (var comment in comments)
            {
               
               commentsViewModels[j].Post = post;
               commentsViewModels[j].User = user;
               commentsViewModels[j].Content = comment.Content;
            }
            postViewModels[i].Comments = commentsViewModels;
            i++;
         }
         userViewModel.Posts = postViewModels;
         var roles = await _roleService.GetRoleByUserId(id);
         userViewModel.Roles = roles;
         return userViewModel;
      }

      [HttpPost]
      [Route("Register")]
      public async Task<IActionResult> Register(RegisterViewModel view)
      {
         var user = _mapper.Map<RegisterViewModel, User>(view);
         var role = await _roleService.GetRoleByName("user");
         await _roleService.Save(user.Id, role.Id);
         await _userService.Save(user);
         return Ok();
      }
      [Authorize(Roles = "user")]
      [HttpPut]
      [Route("Edit")]
      public async Task<IActionResult> Edit(RegisterViewModel view)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var editUser = await _userService.GetUserByEmail(claimEmail);

         if (editUser == null)
            return BadRequest();
         var newUser = _mapper.Map<RegisterViewModel, User>(view);
         await _userService.Update(editUser, newUser);
         return Ok();
      }
      [Authorize(Roles = "user")]
      [HttpDelete]
      [Route("DeleteUser")]
      public async Task<IActionResult> DeleteUser()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);

         if (user == null)
            return BadRequest();
         await _roleService.Delete(user.Id);
         await _userService.Delete(user);
         return Ok();
      }
      [Authorize(Roles = "admin")]
      [HttpDelete]
      [Route("DeleteUser")]
      public async Task<IActionResult> DeleteUser(Guid id)
      {
         var user = await _userService.GetUserById(id);
         if (user == null)
            return BadRequest();
         await _roleService.Delete(user.Id);
         await _userService.Delete(user);
         return Ok();
      }
   }
}
