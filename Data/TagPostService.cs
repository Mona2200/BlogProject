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
      public async Task SaveTagPosts(Guid postId, Guid[] tagIds)
      {
         foreach (var tagId in tagIds)
         {
            var tagPost = new TagPost() { TagId = tagId, PostId = postId };
            var entry = _db.Entry(tagPost);
            if (entry.State == EntityState.Detached)
               await _db.TagPost.AddAsync(tagPost);
         }
      }
      public async Task UpdateTagPosts(Guid postId, Guid[] tagIds)
      {
         await DeleteTagPostByPostId(postId);
         await SaveTagPosts(postId, tagIds);
      }
      public async Task DeleteTagPostByPostId(Guid postId)
      {
         var tagPosts = await GetTagPostByPostId(postId);
         foreach (var tagPost in tagPosts)
         {
            _db.TagPost.Remove(tagPost);
         }
      }
      public async Task DeleteTagPostByTagId(Guid tagId)
      {
         var tagPosts = await GetTagPostByPostId(tagId);
         foreach (var tagPost in tagPosts)
         {
            _db.TagPost.Remove(tagPost);
         }
      }
   }
}
