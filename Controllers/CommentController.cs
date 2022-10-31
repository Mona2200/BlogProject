using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
      [Authorize(Roles = "user")]
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
      [Route("GetAllComments")]
      public async Task<IActionResult> GetAllComments()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);

         var commentsViewModel = await _commentService.GetCommentsViewModelByUserId(user.Id);

         commentsViewModel = commentsViewModel.Reverse().ToArray();
         return View(commentsViewModel);
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("AddComment")]
      public async Task<IActionResult> AddComment(Guid postId)
      {
         var comment = new AddCommentViewModel();
         var post = await _postService.GetPostById(postId);
         var tags = await _tagService.GetTagByPostId(postId);

         var postViewModel = new PostViewModel();
         postViewModel.Id = postId;
         postViewModel.Title = post.Title;
         postViewModel.Content = post.Content;
         postViewModel.Tags = tags;
         postViewModel.Comments = await _commentService.GetCommentsViewModelByPostId(postId);
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
      [HttpGet]
      public async Task<IActionResult> GetEditComment(Guid commentId, Guid postId)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);

         var commentViewModel = new AddCommentViewModel();

         var comment = await GetCommentById(commentId);
         if (comment == null)
            return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
         commentViewModel.Id = comment.Id;
         commentViewModel.Content = comment.Content;

         var post = await _postService.GetPostById(postId);
         var postViewModel = _mapper.Map<Post, PostViewModel>(post);
         postViewModel.User = await _userService.GetUserById(post.UserId);

         var tags = await _tagService.GetTagByPostId(postId);
         postViewModel.Tags = tags;

         postViewModel.Comments = await _commentService.GetCommentsViewModelByPostId(post.Id);
         commentViewModel.Post = postViewModel;

         return View("EditComment", commentViewModel);
      }
      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("EditComment")]
      public async Task<IActionResult> EditComment(AddCommentViewModel view)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);

         var userComments = await _commentService.GetCommentByUserId(user.Id);

         if (userComments.FirstOrDefault(p => p.Id == view.Id) != null)
         {
            var comment = await GetCommentById(view.Id);
            if (comment == null)
               return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найдён" });
            var newComment = _mapper.Map<AddCommentViewModel, Comment>(view);
            await _commentService.Update(comment, newComment);

            return RedirectToAction("AddComment", new { postId = view.Post.Id });
         }
         return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
      }
      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("DeleteComment")]
      public async Task<IActionResult> DeleteComment(Guid commentId, Guid postId)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);

         var userComments = await _commentService.GetCommentByUserId(user.Id);

         if (userComments.FirstOrDefault(p => p.Id == commentId) != null || claimRoles.FirstOrDefault(r => r.Value == "moder") != null)
         {
            var comment = await GetCommentById(commentId);
            if (comment == null)
               return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
            await _commentService.Delete(comment);
            return RedirectToAction("AddComment", new { postId = postId });
         }
         return View("~/Views/Error/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён"});
      }
   }
}
