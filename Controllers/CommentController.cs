using AutoMapper;
using BlogProject.Data;
using BlogProject.Models;
using BlogProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Controllers
{
   public class CommentController : Controller
   {
      private readonly ILogger<CommentController> _logger;
      private readonly IMapper _mapper;
      private readonly CommentService _commentService = new CommentService();
      public CommentController(ILogger<CommentController> logger, IMapper mapper)
      {
         _logger = logger;
         _mapper = mapper;
      }
      [HttpGet]
      [Route("GetComments")]
      public async Task<Comment[]> GetComments()
      {
         var comments = await _commentService.GetComments();
         return comments;
      }
      [HttpGet]
      [Route("GetCommentById")]
      public async Task<Comment> GetCommentById(Guid id)
      {
         var comment = await _commentService.GetCommentById(id);
         return comment;
      }
      [HttpPost]
      [Route("AddComment")]
      public async Task<IActionResult> AddComment(Guid UserId, Guid PostId, CommentViewModel view)
      {
         var comment = _mapper.Map<CommentViewModel, Comment>(view);
         comment.UserId = UserId;
         comment.PostId = PostId;
         await _commentService.Save(comment);
         return StatusCode(200, "Успех");
      }
      [HttpPut]
      [Route("EditComment")]
      public async Task<IActionResult> EditComment(Guid id, CommentViewModel view)
      {
         var comment = await GetCommentById(id);
         if (comment == null)
            return StatusCode(200, "Не Успех");
         var newComment = _mapper.Map<CommentViewModel, Comment>(view);
         await _commentService.Update(comment, newComment);
         return StatusCode(200, "Успех");
      }
      [HttpDelete]
      [Route("DeleteComment")]
      public async Task<IActionResult> DeleteComment(Guid id)
      {
         var comment = await GetCommentById(id);
         if (comment == null)
            return StatusCode(200, "Не Успех");
         await _commentService.Delete(comment);
         return View();
      }
   }
}
