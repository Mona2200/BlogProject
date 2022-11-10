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
         _logger.LogDebug(1, "NLog находится внутри UserController");

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

         return View(userViewModel);
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      public async Task<IActionResult> MainUser(Guid userId)
      {
         var userViewModel = await GetUserById(userId);

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);

         _logger.LogInformation($"Пользователь {user.Id} зашёл в блог пользователя {userId}.");

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
      [Route("GetAllUsers")]
      public async Task<IActionResult> GetAllUsers()
      {
         var users = await _userService.GetUsers();
         var userViewModels = new UserViewModel[users.Length];
         int j = 0;
         foreach (var user in users)
         {
            userViewModels[j] = _mapper.Map<User, UserViewModel>(user);

            var posts = await _postService.GetPostByUserId(user.Id);
            userViewModels[j].Posts = new PostViewModel[posts.Length];

            j++;
         }

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var userIdent = await _userService.GetUserByEmail(claimEmail);

         _logger.LogInformation($"Пользователь {userIdent.Id} просматривает всех пользователей.");

         return View(userViewModels);
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

         var postViewModels = await _postService.GetPostsViewModelByUserId(user.Id);
         userViewModel.Posts = postViewModels.Reverse().ToArray();

         userViewModel.Roles = await _roleService.GetRoleByUserId(id);
         return userViewModel;
      }

      [HttpGet]
      [Route("Register")]
      public IActionResult Register()
      {
         return View();
      }

      [HttpPost]
      [Route("Register")]
      public async Task<IActionResult> Register(RegisterViewModel view)
      {
         if (!ModelState.IsValid)
         {
            foreach (var key in ModelState.Keys)
            {
               if (ModelState[key].Errors.Count > 0)
                  ModelState.AddModelError($"{key}", $"{ModelState[key].Errors[0].ErrorMessage}");
            }

            return View();
         }
            var user = _mapper.Map<RegisterViewModel, User>(view);
            var role = await _roleService.GetRoleByName("user");
            await _roleService.Save(user.Id, role.Id);
            await _userService.Save(user);

            var newUser = await _userService.GetUserByEmail(view.Email);
            var newRoles = await _roleService.GetRoleByUserId(newUser.Id);

            var claims = new List<Claim>
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, newUser.Email)
            };

            for (int i = 0; i < newRoles.Length; i++)
               claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, newRoles[i].Name));

            var claimsIdentity = new ClaimsIdentity(claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            _logger.LogInformation($"Регистрацию прошёл новый пользователь.");

            return RedirectToAction("Main");
         
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("Edit")]
      public async Task<IActionResult> Edit()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var editUser = await _userService.GetUserByEmail(claimEmail);

         if (editUser == null)
         {
            _logger.LogInformation($"Пользователю не удалось открыть форму редактирования своего профиля.");

            return View("~/Views/Errors/Error.cshtml");
         }

         var user = _mapper.Map<User, RegisterViewModel>(editUser);
         return View(user);
      }

      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("Edit")]
      public async Task<IActionResult> Edit(RegisterViewModel view)
      {
         if (!ModelState.IsValid)
         {
            foreach (var key in ModelState.Keys)
            {
               if (ModelState[key].Errors.Count > 0)
                  ModelState.AddModelError($"{key}", $"{ModelState[key].Errors[0].ErrorMessage}");
            }
            return View(view);
         }
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var editUser = await _userService.GetUserByEmail(claimEmail);

         if (editUser == null)
         {
            _logger.LogInformation($"Пользователю не удалось отредактировать свой профиль.");

            return View("~/Views/Errors/Error.cshtml");
         }

         var newUser = _mapper.Map<RegisterViewModel, User>(view);
         await _userService.Update(editUser, newUser);

         var oldClaimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name);
         ident.RemoveClaim(oldClaimEmail);

         var newClaimEmail = new Claim(ClaimsIdentity.DefaultNameClaimType, newUser.Email);

         ident.AddClaim(newClaimEmail);
         var claimsIdentity = new ClaimsIdentity(ident.Claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
         await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

         _logger.LogInformation($"Профиль пользователя {editUser.Id} обновлён.");

         return RedirectToRoute(new { controller = "User", action = "Main" });
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("DeleteUser")]
      public async Task<IActionResult> DeleteUser()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);

         if (user == null)
         {
            _logger.LogInformation($"Пользователю не удалось удалить свой профиль.");

            return View("~/Views/Errors/Error.cshtml");
         }

         await _roleService.Delete(user.Id);
         await _userService.Delete(user);

         await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

         _logger.LogInformation($"Пользователь {user.Id} удалён.");

         return RedirectToAction("Index", "Home");
      }

      [Authorize(Roles = "admin")]
      [HttpGet]
      [Route("DeleteUserByAdmin")]
      public async Task<IActionResult> DeleteUserByAdmin(Guid id)
      {
         var user = await _userService.GetUserById(id);
         if (user == null)
         {
            _logger.LogInformation($"Администратору не удалось удалить пользователя {user.Id}.");

            return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
         }

         await _roleService.Delete(user.Id);
         await _userService.Delete(user);

         _logger.LogInformation($"Администратор удалил пользователя {user.Id}.");

         return RedirectToAction("GetAllUsers");
      }
   }
}
