using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Security.Claims;

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
         _mapper = mapper;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetPosts")]
      public async Task<IActionResult> GetPosts()
      {
         var posts = await _postService.GetPosts();
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);
            postViewModels[i].Tags = await _tagService.GetTagByPostId(post.Id);          
            postViewModels[i].Comments = await _commentService.GetCommentsViewModelByPostId(post.Id);
            postViewModels[i].User = await _userService.GetUserById(post.UserId);

            i++;
         }

         postViewModels = postViewModels.Reverse().ToArray();

         var getPosts = new GetPostsViewModel() { posts = postViewModels };

         return View(getPosts);
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetPostById")]
      public async Task<PostViewModel> GetPostById(Guid id)
      {
         var post = await _postService.GetPostById(id);
         if (post == null)
            return null;
         var postViewModel = _mapper.Map<Post, PostViewModel>(post);
         postViewModel.Tags = await _tagService.GetTagByPostId(post.Id);

         var commentsViewModels = await _commentService.GetCommentsViewModelByPostId(post.Id);

         postViewModel.Comments = commentsViewModels;
         return postViewModel;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetPostByUserId")]
      public async Task<PostViewModel[]> GetPostByUserId(Guid id)
      {
         var posts = await _postService.GetPostByUserId(id);
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);
            postViewModels[i].Tags = await _tagService.GetTagByPostId(post.Id);
            postViewModels[i].Comments = await _commentService.GetCommentsViewModelByPostId(post.Id);

            i++;
         }
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
            post.Post = new AddPostViewModel();

            post.postId = modelPost.Id;
            post.Post.Title = modelPost.Title;
            post.Post.Content = modelPost.Content;
            post.AllTags = await _tagService.GetTags();

            var tagsPost = await _tagService.GetTagByPostId(modelPost.Id);

            foreach (var tag in tagsPost.Select(t => t.Id).ToArray())
            {
               post.TagIds += tag + " ";
            }

            return View(post);
         }
         else
            return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
      }
      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(FormPostViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);
         var userPosts = await _postService.GetPostByUserId(user.Id);

         if (userPosts.FirstOrDefault(p => p.Id == model.postId) != null)
         {
            var editPost = await _postService.GetPostById(model.postId);
            if (editPost == null)
               return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
            var newPost = _mapper.Map<AddPostViewModel, Post>(model.Post);

            Guid[] tagIds = model.TagIds.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(t => Guid.Parse(t)).ToArray();

            await _postService.Update(editPost, newPost, tagIds);
            return RedirectToAction("Main", "User");
         }
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
               return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
            await _postService.Delete(post);
            return RedirectToAction("Main", "User");
         }
         return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
      }
   }
}
