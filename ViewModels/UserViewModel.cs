using BlogProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels
{
   public class UserViewModel
   {
      public Guid Id { get; set; }
      public string FullName { get; set; }
      public string Sex { get; set; }
      public string Email { get; set; }
      public Role[] Roles { get; set; }
      public PostViewModel[] Posts { get; set; }
   }
}
