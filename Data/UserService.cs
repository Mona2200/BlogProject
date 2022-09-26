using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogProject.Data
{
   public class UserService
   {
      private readonly BlogDbContext _db = new BlogDbContext();
      public async Task<User[]> GetUsers()
      {
         var users = await _db.User.ToArrayAsync();
         return users;
      }
      public async Task<User> GetUserById(Guid id)
      {
         var user = await _db.User.Where(u => u.Id == id).FirstOrDefaultAsync();
         return user;
      }
      public async Task Save(User user)
      {
         var entry = _db.Entry(user);
         if (entry.State == EntityState.Detached)
            await _db.User.AddAsync(user);
         await _db.SaveChangesAsync();
      }
      public async Task Update(User updateUser, User newUser)
      {
         updateUser.FirstName = newUser.FirstName;
         updateUser.LastName = newUser.LastName;
         updateUser.Sex = newUser.Sex;
         updateUser.Email = newUser.Email;
         updateUser.Password = newUser.Password;
         var entry = _db.Entry(updateUser);
         if (entry.State == EntityState.Detached)
            _db.User.Update(updateUser);
         await _db.SaveChangesAsync();
      }
      public async Task Delete(User user)
      {
         _db.User.Remove(user);
         await _db.SaveChangesAsync();
      }
   }
}
