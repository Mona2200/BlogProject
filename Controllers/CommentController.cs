using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels.Request;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.Design;
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
         _logger.LogDebug(1, "NLog находится внутри CommentController");

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

         _logger.LogInformation($"Пользователь {user.Id} просматривает свои комментарии.");

         return View(commentsViewModel);
      }

      [Authorize(Roles = "user")]
      [HttpGet]
      [Route("AddComment")]
      public async Task<IActionResult> AddComment(Guid postId)
      {
         var comment = new AddCommentViewModel();
         comment.Post = await _postService.GetPostViewModelById(postId);
         return View(comment);
      }

      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("AddComment")]
      public async Task<IActionResult> AddComment(AddCommentViewModel view)
      {
         if (ModelState["Content"]?.Errors.Count > 0)
         {
            ModelState.AddModelError("Content", $"{ModelState["Content"].Errors[0].ErrorMessage}");
            view.Post = await _postService.GetPostViewModelById(view.Post.Id);
            return View(view);
         }

         var postId = view.Post.Id;

         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _userService.GetUserByEmail(claimEmail);
         var userId = user.Id;

         var comment = _mapper.Map<AddCommentViewModel, Comment>(view);
         comment.UserId = userId;
         comment.PostId = postId;
         await _commentService.Save(comment);

         _logger.LogInformation($"Пользователь {userId} добавляет комментарий к публикации {postId}.");

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

         var comment = await GetCommentById(commentId);
         if (comment == null)
         {
            _logger.LogInformation($"Пользователю {user.Id} не удалось открыть форму редактирования комментария {commentId}, т.к. комментарий не был найден.");

            return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
         }
   
         var commentViewModel = _mapper.Map<Comment, AddCommentViewModel>(comment);
         commentViewModel.Post = await _postService.GetPostViewModelById(postId);

         return View("EditComment", commentViewModel);
      }

      [Authorize(Roles = "user")]
      [HttpPost]
      [Route("EditComment")]
      public async Task<IActionResult> EditComment(AddCommentViewModel view)
      {
         if (ModelState["Content"]?.Errors.Count > 0)
         {
            ModelState.AddModelError("Content", $"{ModelState["Content"].Errors[0].ErrorMessage}");
            var comment = await GetCommentById(view.Id);
            var commentViewModel = _mapper.Map<Comment, AddCommentViewModel>(comment);
            commentViewModel.Post = await _postService.GetPostViewModelById(view.Post.Id);
            return View(commentViewModel);
         }
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var claimRoles = ident.Claims.Where(u => u.Type == ClaimTypes.Role).ToArray();
         var user = await _userService.GetUserByEmail(claimEmail);

         var userComments = await _commentService.GetCommentByUserId(user.Id);

         if (userComments.FirstOrDefault(p => p.Id == view.Id) != null)
         {
            var comment = await GetCommentById(view.Id);
            if (comment == null)
            {
               _logger.LogInformation($"Пользователю {user.Id} не удалось отредактировать комментарий {view.Id}, т.к. комментарий не был найден.");
               
               return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найдён" });
            }

            var newComment = _mapper.Map<AddCommentViewModel, Comment>(view);
            await _commentService.Update(comment, newComment);

            _logger.LogInformation($"Пользователь {user.Id} редактирует свой комментарий {comment.Id}.");

            return RedirectToAction("AddComment", new { postId = view.Post.Id });
         }
         _logger.LogInformation($"Пользователю {user.Id} не удалось отредактировать комментарий {view.Id}, т.к. комментарий не был найден среди комментарией этого пользователя.");
         
         return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
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
            {
               _logger.LogInformation($"Пользователю {user.Id} не удалось удалить комментарий {commentId}, т. к. комментарий не был найден.");

               return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Ресурс не найден" });
            }

            await _commentService.Delete(comment);

            _logger.LogInformation($"Пользователь {user.Id} удалил комментарий {comment.Id}.");

            return RedirectToAction("AddComment", new { postId = postId });
         }
         _logger.LogInformation($"Пользователю {user.Id} не удалось удалить комментарий {commentId}, т. к. комментарий не был найден среди комментариев пользователя и он не является модератором.");

         return View("~/Views/Errors/Error.cshtml", new ErrorViewModel() { ErrorMessage = "Доступ запрещён" });
      }
   }
}
