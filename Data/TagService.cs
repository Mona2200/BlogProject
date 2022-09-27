using BlogProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data
{
   public class TagService
   {
   private readonly BlogDbContext _db = new BlogDbContext();
   private readonly TagPostService _tagPostService = new TagPostService();
      public async Task<Tag[]> GetTags()
      {
         var tags = await _db.Tag.ToArrayAsync();
         return tags;
      }
      public async Task<Tag> GetTagById(Guid id)
      {
         var tag = await _db.Tag.Where(c => c.Id == id).FirstOrDefaultAsync();
         return tag;
      }
      public async Task Save(Tag tag)
      {
         var entry = _db.Entry(tag);
         if (entry.State == EntityState.Detached)
            await _db.Tag.AddAsync(tag);
         await _db.SaveChangesAsync();
      }
      public async Task Update(Tag updateTag, Tag newTag)
      {
         updateTag.Name = newTag.Name;
         var entry = _db.Entry(updateTag);
         if (entry.State == EntityState.Detached)
            _db.Tag.Update(updateTag);
         await _db.SaveChangesAsync();
      }
      public async Task Delete(Tag tag)
      {
         _db.Tag.Remove(tag);
         var tagPosts = await _tagPostService.GetTagPostByTagId(tag.Id);
         foreach (var tagPost in tagPosts)
         {
            await _tagPostService.Delete(tagPost);
         }
         await _db.SaveChangesAsync();
      }
   }
}
