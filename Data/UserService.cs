using BlogProject.Models;
using BlogProject.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogProject.Data
{
   public class UserService
   {
      private readonly BlogDbContext _db = new BlogDbContext();
      private readonly PostService _postService = new PostService();
      private readonly CommentService _commentService = new CommentService();
      private readonly RoleService _roleService = new RoleService();

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

      public async Task<User> GetUserByEmail(string email)
      {
         var user = await _db.User.Where(u => u.Email == email).FirstOrDefaultAsync();
         return user;
      }

      private async Task<UserViewModel> FromUserToUserViewModel(User user)
      {
         var userViewModel = new UserViewModel();
         userViewModel.Id = user.Id;
         userViewModel.FullName = user.FirstName + " " + user.LastName;
         userViewModel.Sex = user.Sex;
         userViewModel.Email = user.Email;
         userViewModel.Roles = await _roleService.GetRoleByUserId(user.Id);
         userViewModel.Posts = await _postService.GetPostsViewModelByUserId(user.Id);

         return userViewModel;
      }

      private async Task<UserViewModel[]> FromUsersToUserViewModels(User[] users)
      {
         var userViewModels = new UserViewModel[users.Length];
         int i = 0;
         foreach (var user in users)
         {
            userViewModels[i] = await FromUserToUserViewModel(user);
            i++;
         }

         return userViewModels;
      }

      public async Task<UserViewModel[]> GetUserViewModels()
      {
         var users = await _db.User.ToArrayAsync();
         var userViewModels = await FromUsersToUserViewModels(users);
         return userViewModels;
      }

      public async Task<UserViewModel> GetUserViewModelById(Guid id)
      {
         var user = await _db.User.Where(u => u.Id == id).FirstOrDefaultAsync();
         var userViewModel = await FromUserToUserViewModel(user);
         return userViewModel;
      }

      public async Task<UserViewModel> GetUserViewModelByEmail(string email)
      {
         var user = await _db.User.Where(u => u.Email == email).FirstOrDefaultAsync();
         var userViewModel = await FromUserToUserViewModel(user);
         return userViewModel;
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
         if (!String.IsNullOrEmpty(newUser.Password))
            updateUser.Password = newUser.Password;
         var entry = _db.Entry(updateUser);
         if (entry.State == EntityState.Detached)
            _db.User.Update(updateUser);
         await _db.SaveChangesAsync();
      }

      public async Task Delete(User user)
      {
         _db.User.Remove(user);
         var posts = await _postService.GetPostByUserId(user.Id);
         foreach (var post in posts)
         {
            await _postService.Delete(post);
         }
         var comments = await _commentService.GetCommentByUserId(user.Id);
         foreach (var comment in comments)
         {
            await _commentService.Delete(comment);
         }
         await _db.SaveChangesAsync();
      }
   }
}
