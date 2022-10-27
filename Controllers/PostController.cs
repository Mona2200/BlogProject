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

            var comments = await _commentService.GetCommentByPostId(post.Id);
            var commentsViewModels = new CommentViewModel[comments.Length];
            var j = 0;
            foreach (var comment in comments)
            {
               commentsViewModels[j] = new CommentViewModel();
               commentsViewModels[j].Id = comment.Id;
               commentsViewModels[j].Post = post;
               commentsViewModels[j].User = await _userService.GetUserById(comment.UserId);
               commentsViewModels[j].Content = comment.Content;
               j++;
            }
            postViewModels[i].Comments = commentsViewModels;
            postViewModels[i].User = await _userService.GetUserById(post.UserId);

            i++;
         }

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

         var comments = await _commentService.GetCommentByPostId(post.Id);
         var commentsViewModels = new CommentViewModel[comments.Length];
         var j = 0;
         foreach (var comment in comments)
         {

            commentsViewModels[j].Post = post;
            commentsViewModels[j].User = await _userService.GetUserById(comment.UserId);
            commentsViewModels[j].Content = comment.Content;
            j++;
         }

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

            var comments = await _commentService.GetCommentByPostId(post.Id);
            var commentsViewModels = new CommentViewModel[comments.Length];
            var j = 0;
            foreach (var comment in comments)
            {

               commentsViewModels[j].Post = post;
               commentsViewModels[j].User = await _userService.GetUserById(comment.UserId);
               commentsViewModels[j].Content = comment.Content;
               j++;
            }
            postViewModels[i].Comments = commentsViewModels;

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
            return RedirectToAction("Main", "User");
      }
      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(/*Guid id, Guid[] TagIds, AddPostViewModel view*/FormPostViewModel model)
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
               return BadRequest();
            var newPost = _mapper.Map<AddPostViewModel, Post>(model.Post);

            Guid[] tagIds = model.TagIds.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(t => Guid.Parse(t)).ToArray();

            await _postService.Update(editPost, newPost, tagIds);
            return RedirectToAction("Main", "User");
         }
         return BadRequest();
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
               return BadRequest();
            await _postService.Delete(post);
            return RedirectToAction("Main", "User");
         }
         return BadRequest();
      }
   }
}
