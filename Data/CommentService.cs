using BlogProject.Models;
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
      public async Task DeleteByUserId(Guid userId)
      {
         var comments = await GetCommentByUserId(userId);
         foreach (var comment in comments)
         {
         await Delete(comment);
         }
      }
   }
}
