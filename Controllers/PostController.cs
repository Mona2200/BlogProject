using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlogProject.Controllers
{
   public class PostController : Controller
   {
      private readonly ILogger<PostController> _logger;
      private readonly IMapper _mapper;
      private readonly UserService _userService = new UserService();
      private readonly PostService _postService = new PostService();
      private readonly TagService _tagService = new TagService();
      private readonly CommentService _commentService = new CommentService();

      public PostController(ILogger<PostController> logger, IMapper mapper)
      {
         _logger = logger;
         _logger.LogDebug(1, "NLog находится внутри PostController");

         _mapper = mapper;
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetPosts")]
      public async Task<IActionResult> GetPosts()
      {
         var postViewModels = await _postService.GetPostsViewModelAll();

         var getPosts = new GetPostsViewModel() { posts = postViewModels.Reverse().ToArray() };


         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);

         _logger.LogInformation($"Пользователь {user.Id} просматривает все публикации.");

         return View(getPosts);
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetPostById")]
      public async Task<PostViewModel> GetPostById(Guid id)
      {
         var postViewModel = await _postService.GetPostViewModelById(id);
         return postViewModel;
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetPostByUserId")]
      public async Task<PostViewModel[]> GetPostByUserId(Guid id)
      {
         var postViewModels = await _postService.GetPostsViewModelByUserId(id);
         return postViewModels;
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("AddPost")]
      public async Task<IActionResult> AddPost()
      {
         return View();
      }

      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("AddPost")]
      public async Task<IActionResult> AddPost(FormPostViewModel model)
      {
         var view = model.Post;

         Guid[] tagIds = model.TagIds.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => Guid.Parse(x)).ToArray();

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);
         var userId = user.Id;

         var post = _mapper.Map<AddPostViewModel, Post>(view);
         post.UserId = userId;
         await _postService.Save(post, tagIds);

         _logger.LogInformation($"Пользователь {user.Id} добавляет новую публикацию.");

         return RedirectToAction("Main", "User");
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(Guid postId)
      {
         var modelPost = await _postService.GetPostById(postId);

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);
         var tryUser = await _userService.GetUserById(modelPost.UserId);

         if (user == tryUser)
         {
            var post = new FormPostViewModel();
            post.Post = _mapper.Map<Post, AddPostViewModel>(modelPost);
            post.postId = modelPost.Id;
            post.AllTags = await _tagService.GetTags();

            var tagsPost = await _tagService.GetTagByPostId(modelPost.Id);

            foreach (var tag in tagsPost.Select(t => t.Id).ToArray())
            {
               post.TagIds += tag + " ";
            }

            _logger.LogInformation($"Пользователь {user.Id} открывает форму редактирования публикации {postId}.");

            return View(post);
         }
         else
         {
            _logger.LogInformation($"Пользователю {user.Id} не удалось открыть форму редактирования публикации {postId}, т. к. он не является её создателем.");

            return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
         }
      }

      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(FormPostViewModel model)
      {
         if (ModelState["Post.Title"].Errors.Count > 0)
         {
            ModelState.AddModelError("Post.Title", $"{ModelState["Post.Title"].Errors[0].ErrorMessage}");
            model.AllTags = await _tagService.GetTags();
            return View(model);
         }

         if (ModelState["Post.Content"].Errors.Count > 0)
         {
            ModelState.AddModelError("Post.Content", $"{ModelState["Post.Content"].Errors[0].ErrorMessage}");
            model.AllTags = await _tagService.GetTags();
            return View(model);
         }

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);
         var userPosts = await _postService.GetPostByUserId(user.Id);

         if (userPosts.FirstOrDefault(p => p.Id == model.postId) != null)
         {
            var editPost = await _postService.GetPostById(model.postId);
            if (editPost == null)
            {
               _logger.LogInformation($"Пользователю {user.Id} не удалось отредактировать публикацию {model.postId}, т. к. публикация не была найдена.");

               return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
            }

            var newPost = _mapper.Map<AddPostViewModel, Post>(model.Post);

            Guid[] tagIds = model.TagIds.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(t => Guid.Parse(t)).ToArray();

            await _postService.Update(editPost, newPost, tagIds);

            _logger.LogInformation($"Пользователь {user.Id} редактирует свою публикацию {model.postId}.");

            return RedirectToAction("Main", "User");
         }
         _logger.LogInformation($"Пользователю {user.Id} не удалось отредактировать публикацию {model.postId}, т. к. публикация не была найдена среди публикаций пользователя.");

         return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("DeletePost")]
      public async Task<IActionResult> DeletePost(Guid id)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);
         var userPosts = await _postService.GetPostByUserId(user.Id);

         if (userPosts.FirstOrDefault(p => p.Id == id) != null || claimRoles.FirstOrDefault(r => r.Value == "moder") != null)
         {
            var post = await _postService.GetPostById(id);
            if (post == null)
            {
               _logger.LogInformation($"Пользователю {user.Id} не удалось удалить публикацию {id}, т. к. публикация не была найдена.");

               return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
            }

            await _postService.Delete(post);

            _logger.LogInformation($"Пользователь {user.Id} удалил публикацию {id}.");

            return RedirectToAction("Main", "User");
         }
         _logger.LogInformation($"Пользователю {user.Id} не удалось удалить публикацию {id}, т. к. публикация не была найдена среди его публикаций и он не является модератором.");

         return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
      }
   }
}
