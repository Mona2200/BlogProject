using BlogProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data
{
   public class RoleService
   {
      private readonly BlogDbContext _db = new BlogDbContext();

      public async Task<Role[]> GetRoles()
      {
         return await _db.Role.ToArrayAsync();
      }

      public async Task<Role[]> GetRoleByUserId(Guid userId)
      {
         var userRoles = await _db.UserRole.Where(u => u.UserId == userId).ToArrayAsync();
         var roles = new Role[userRoles.Length];
         int i = 0;
         foreach (var userRole in userRoles)
         {
            roles[i] = await _db.Role.Where(r => r.Id == userRole.RoleId).FirstOrDefaultAsync();
            i++;
         }
         return roles;
      }

      public async Task<Role> GetRoleByName(string name)
      {
         var role = await _db.Role.Where(r => r.Name == name).FirstOrDefaultAsync();
         return role;
      }

      public async Task Create(Role role)
      {
         await _db.Role.AddAsync(role);
         await _db.SaveChangesAsync();
      }

      public async Task Save(Guid userId, Guid roleId)
      {
         var userRole = new UserRole() { UserId = userId, RoleId = roleId };
         await _db.UserRole.AddAsync(userRole);
         await _db.SaveChangesAsync();
      }

      public async Task Delete(Guid userId)
      {
         var userRoles = await _db.UserRole.Where(u => u.UserId == userId).ToArrayAsync();
         foreach (var userRole in userRoles)
         {
            _db.UserRole.Remove(userRole);
         }
         await _db.SaveChangesAsync();
      }
   }
}
