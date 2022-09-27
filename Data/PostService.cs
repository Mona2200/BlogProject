using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data
{
   public class PostService
   {
      private readonly BlogDbContext _db = new BlogDbContext();
      private readonly TagPostService _tagPostService = new TagPostService();
      private readonly CommentService _commentService = new CommentService();
      public async Task<Post[]> GetPosts()
      {
         var posts = await _db.Post.ToArrayAsync();
         return posts;
      }
      public async Task<Post> GetPostById(Guid id)
      {
         var post = await _db.Post.Where(p => p.Id == id).FirstOrDefaultAsync();
         return post;
      }
      public async Task<Post[]> GetPostByUserId(Guid id)
      {
         var post = await _db.Post.Where(p => p.UserId == id).ToArrayAsync();
         return post;
      }
      public async Task Save(Post post, Guid[] tagIds)
      {
         var entry = _db.Entry(post);
         if (entry.State == EntityState.Detached)
            await _db.Post.AddAsync(post);
         await _tagPostService.Save(post.Id, tagIds);
         await _db.SaveChangesAsync();
      }
      public async Task Update(Post updatePost, Post newPost, Guid[] tagIds)
      {
         updatePost.Title = newPost.Title;
         updatePost.Content = newPost.Content;

         var entry = _db.Entry(updatePost);
         if (entry.State == EntityState.Detached)
            _db.Post.Update(updatePost);
         await _tagPostService.Update(updatePost.Id, tagIds);
         await _db.SaveChangesAsync();
      }
      public async Task Delete(Post post)
      {
         _db.Post.Remove(post);
         var tagPosts = await _tagPostService.GetTagPostByPostId(post.Id);
         foreach (var tagPost in tagPosts)
         {
            await _tagPostService.Delete(tagPost);
         }
         var comments = await _commentService.GetCommentByPostId(post.Id);
         foreach (var comment in comments)
         {
            await _commentService.Delete(comment);
         }
         await _db.SaveChangesAsync();
      }
   }
}
