using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogProject.Controllers
{
    public class CommentController : Controller
   {
      private readonly ILogger<CommentController> _logger;
      private readonly IMapper _mapper;
      private readonly UserService _userService = new UserService();
      private readonly CommentService _commentService = new CommentService();
      private readonly PostService _postService = new PostService();
      private readonly TagService _tagService = new TagService();
      public CommentController(ILogger<CommentController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }
      [Authorize(Roles = "moder")]
      [HttpGet]
      [Route("GetComments")]
      public async Task<Comment[]> GetComments()
      {
         var comments = await _commentService.GetComments();
         return comments;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("GetCommentById")]
      public async Task<Comment> GetCommentById(Guid id)
      {
         var comment = await _commentService.GetCommentById(id);
         return comment;
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("AddComment")]
      public async Task<IActionResult> AddComment(Guid postId)
      {
         var comment = new AddCommentViewModel();
         var post = await _postService.GetPostById(postId);
         var tags = await _tagService.GetTagByPostId(postId);

         var comments = await _commentService.GetCommentByPostId(post.Id);
         var commentsViewModels = new CommentViewModel[comments.Length];
         var j = 0;
         foreach (var comm in comments)
         {
            commentsViewModels[j] = new CommentViewModel();
            commentsViewModels[j].Post = post;
            commentsViewModels[j].User = await _userService.GetUserById(comm.UserId);
            commentsViewModels[j].Content = comm.Content;
            j++;
         }

         var postViewModel = new PostViewModel();
         postViewModel.Id = postId;
         postViewModel.Title = post.Title;
         postViewModel.Content = post.Content;
         postViewModel.Tags = tags;
         postViewModel.Comments = commentsViewModels;
         comment.Post = postViewModel;
         return View(comment);
      }
      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("AddComment")]
      public async Task<IActionResult> AddComment(AddCommentViewModel view)
      {
         var postId = view.Post.Id;
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);
         var userId = user.Id;

         var comment = _mapper.Map<AddCommentViewModel, Comment>(view);
         comment.UserId = userId;
         comment.PostId = postId;
         await _commentService.Save(comment);
         return RedirectToAction("AddComment", "Comment", new { postId = postId });
      }
      [Authorize(Roles = "user")]
      [HttpPut]
      [Route("EditComment")]
      public async Task<IActionResult> EditComment(Guid id, AddCommentViewModel view)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);
         var userComments = await _commentService.GetCommentByUserId(user.Id);

         if (userComments.FirstOrDefault(p => p.Id == id) != null || claimRoles.FirstOrDefault(r => r.Value == "moder") != null)
         {
            var comment = await GetCommentById(id);
            if (comment == null)
               return BadRequest();
            var newComment = _mapper.Map<AddCommentViewModel, Comment>(view);
            await _commentService.Update(comment, newComment);
            return Ok();
         }
         return BadRequest();
      }
      [Authorize(Roles = "user")]
      [HttpDelete]
      [Route("DeleteComment")]
      public async Task<IActionResult> DeleteComment(Guid id)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);
         var userComments = await _commentService.GetCommentByUserId(user.Id);

         if (userComments.FirstOrDefault(p => p.Id == id) != null || claimRoles.FirstOrDefault(r => r.Value == "moder") != null)
         {
            var comment = await GetCommentById(id);
            if (comment == null)
               return BadRequest();
            await _commentService.Delete(comment);
            return Ok();
         }
         return BadRequest();
      }
   }
}
