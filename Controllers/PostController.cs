using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
      public async Task<PostViewModel[]> GetPosts()
      {
         var posts = await _postService.GetPosts();
         var postViewModels = new PostViewModel[posts.Length];
         int i = 0;
         foreach (var post in posts)
         {
            postViewModels[i] = _mapper.Map<Post, PostViewModel>(post);
            postViewModels[i].Tags = await _tagService.GetTagByPostId(post.Id);
            postViewModels[i].Comments = await _commentService.GetCommentByPostId(post.Id);
            i++;
         }
         return postViewModels;
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
         postViewModel.Comments = await _commentService.GetCommentByPostId(post.Id);
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
            postViewModels[i].Comments = await _commentService.GetCommentByPostId(post.Id);
            i++;
         }
         return postViewModels;
      }
      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("AddPost")]
      public async Task<IActionResult> AddPost(Guid[] tagIds, AddPostViewModel view)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);
         var userId = user.Id;

         var post = _mapper.Map<AddPostViewModel, Post>(view);
         post.UserId = userId;
         await _postService.Save(post, tagIds);
         return Ok();
      }
      [Authorize(Roles = "user")]
      [HttpPut]
      [Route("EditPost")]
      public async Task<IActionResult> EditPost(Guid id, Guid[] tagIds, AddPostViewModel view)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);
         var userPosts = await _postService.GetPostByUserId(user.Id);

         if (userPosts.FirstOrDefault(p => p.Id == id) != null || claimRoles.FirstOrDefault(r => r.Value == "moder") != null)
         {
            var editPost = await _postService.GetPostById(id);
            if (editPost == null)
               return BadRequest();
            var newPost = _mapper.Map<AddPostViewModel, Post>(view);
            await _postService.Update(editPost, newPost, tagIds);
            return Ok();
         }
         return BadRequest();
      }
      [Authorize(Roles = "user")]
      [HttpDelete]
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
            return Ok();
         }
         return BadRequest();
      }
   }
}
