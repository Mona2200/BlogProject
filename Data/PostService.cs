using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data
{
   public class PostService
   {
      private readonly BlogDbContext _db = new BlogDbContext();
      private readonly TagPostService _tagPostService = new TagPostService();
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
         await _tagPostService.SaveTagPosts(post.Id, tagIds);
         await _db.SaveChangesAsync();
      }
      public async Task Update(Post updatePost, Post newPost, Guid[] tagIds)
      {
         updatePost.Title = newPost.Title;
         updatePost.Content = newPost.Content;

         var entry = _db.Entry(updatePost);
         if (entry.State == EntityState.Detached)
            _db.Post.Update(updatePost);
         await _tagPostService.UpdateTagPosts(updatePost.Id, tagIds);
         await _db.SaveChangesAsync();
      }
      public async Task Delete(Post post)
      {
         _db.Post.Remove(post);
         await _tagPostService.DeleteTagPostByPostId(post.Id);
         await _db.SaveChangesAsync();
      }
      public async Task DeleteByUserId(Guid userId)
      {
         var posts = await GetPostByUserId(userId);
         foreach (var post in posts)
         {
            await Delete(post);
         }
      }
   }
}
