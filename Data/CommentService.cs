using BlogProject.Models;
using BlogProject.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BlogProject.Data
{
   public class CommentService
   {
      private readonly BlogDbContext _db = new BlogDbContext();

      public async Task<Comment[]> GetComments()
      {
         var comments = await _db.Comment.ToArrayAsync();
         return comments;
      }

      public async Task<Comment> GetCommentById(Guid id)
      {
         var comment = await _db.Comment.Where(c => c.Id == id).FirstOrDefaultAsync();
         return comment;
      }

      public async Task<Comment[]> GetCommentByUserId(Guid id)
      {
         var comment = await _db.Comment.Where(c => c.UserId == id).ToArrayAsync();
         return comment;
      }

      public async Task<Comment[]> GetCommentByPostId(Guid id)
      {
         var comment = await _db.Comment.Where(c => c.PostId == id).ToArrayAsync();
         return comment;
      }

      private async Task<CommentViewModel[]> FromCommentsToCommentViewModels(Comment[] comments)
      {
         var commentsViewModels = new CommentViewModel[comments.Length];
         var j = 0;
         foreach (var comm in comments)
         {
            commentsViewModels[j] = await FromCommentToCommentViewModel(comm);
            j++;
         }
         return commentsViewModels;
      }

      private async Task<CommentViewModel> FromCommentToCommentViewModel(Comment comment)
      {
            var commentsViewModels = new CommentViewModel();
            commentsViewModels.Post = await _db.Post.Where(p => p.Id == comment.PostId).FirstOrDefaultAsync();
            commentsViewModels.User = await _db.User.Where(u => u.Id == comment.UserId).FirstOrDefaultAsync();

            commentsViewModels.Id = comment.Id;
            commentsViewModels.Content = comment.Content;
         return commentsViewModels;
      }

      public async Task<CommentViewModel[]> GetCommentsViewModel()
      {
         var comments = await GetComments();
         var commentsViewModels = await FromCommentsToCommentViewModels(comments);
         return commentsViewModels;
      }

      public async Task<CommentViewModel> GetCommentViewModelById(Guid id)
      {
         var comment = await GetCommentById(id);
         var commentsViewModel = await FromCommentToCommentViewModel(comment);
         return commentsViewModel;
      }

      public async Task<CommentViewModel[]> GetCommentsViewModelByPostId(Guid postId)
      {
         var comments = await GetCommentByPostId(postId);
         var commentsViewModels = await FromCommentsToCommentViewModels(comments);
         return commentsViewModels;
      }

      public async Task<CommentViewModel[]> GetCommentsViewModelByUserId(Guid userId)
      {
         var comments = await GetCommentByUserId(userId);
         var commentsViewModels = await FromCommentsToCommentViewModels(comments);
         return commentsViewModels;
      }

      public async Task Save(Comment comment)
      {
         var entry = _db.Entry(comment);
         if (entry.State == EntityState.Detached)
            await _db.Comment.AddAsync(comment);
         await _db.SaveChangesAsync();
      }

      public async Task Update(Comment updateComment, Comment newComment)
      {
         updateComment.Content = newComment.Content;
         var entry = _db.Entry(updateComment);
         if (entry.State == EntityState.Detached)
            _db.Comment.Update(updateComment);
         await _db.SaveChangesAsync();
      }

      public async Task Delete(Comment comment)
      {
         _db.Comment.Remove(comment);
         await _db.SaveChangesAsync();
      }
   }
}
