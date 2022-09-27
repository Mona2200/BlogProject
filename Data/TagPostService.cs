using BlogProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace BlogProject.Data
{
   public class TagPostService
   {
      private readonly BlogDbContext _db = new BlogDbContext();
      public async Task<TagPost[]> GetTagPostByPostId(Guid id)
      {
         var tagPosts = await _db.TagPost.Where(tp => tp.PostId == id).ToArrayAsync();
         return tagPosts;
      }
      public async Task<TagPost[]> GetTagPostByTagId(Guid id)
      {
         var tagPosts = await _db.TagPost.Where(tp => tp.TagId == id).ToArrayAsync();
         return tagPosts;
      }
      public async Task Save(Guid postId, Guid[] tagIds)
      {
         foreach (var tagId in tagIds)
         {
            var tagPost = new TagPost() { TagId = tagId, PostId = postId };
            var entry = _db.Entry(tagPost);
            if (entry.State == EntityState.Detached)
               await _db.TagPost.AddAsync(tagPost);
         }
         await _db.SaveChangesAsync();
      }
      public async Task Update(Guid postId, Guid[] tagIds)
      {
         var tagPosts = await GetTagPostByPostId(postId);
         foreach (var tagPost in tagPosts)
         {
            await Delete(tagPost);
         }
         await Save(postId, tagIds);

      }
      public async Task Delete(TagPost tagPost)
      {
         _db.TagPost.Remove(tagPost);
         await _db.SaveChangesAsync();
      }
   }
}
