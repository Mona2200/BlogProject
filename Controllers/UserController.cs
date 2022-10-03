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
      private readonly PostService _postService = new PostService();
      private readonly TagService _tagService = new TagService();
      private readonly CommentService _commentService = new CommentService();
      private readonly RoleService _roleService = new RoleService();
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
            postViewModels[i].Comments = await _commentService.GetCommentByPostId(post.Id);
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
         return StatusCode(200, "Успех");
      }
      [HttpPut]
      [Route("Edit")]
      public async Task<IActionResult> Edit(Guid id, RegisterViewModel view)
      {
         var editUser = await _userService.GetUserById(id);
         if (editUser == null)
            return StatusCode(200, "Не Успех");
         var newUser = _mapper.Map<RegisterViewModel, User>(view);
         await _userService.Update(editUser, newUser);
         return StatusCode(200, "Успех");
      }
      [HttpDelete]
      [Route("DeleteUser")]
      public async Task<IActionResult> DeleteUser(Guid id)
      {
         var user = await _userService.GetUserById(id);
         if (user == null)
            return StatusCode(200, "Не Успех");
         await _roleService.Delete(user.Id);
         await _userService.Delete(user);
         return StatusCode(200, "Успех");
      }
   }
}
