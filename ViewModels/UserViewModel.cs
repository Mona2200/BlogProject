using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels
{
   public class UserViewModel
   {
      public Guid Id { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public string Password { get; set; }
      public string Sex { get; set; }
      public string Email { get; set; }
      public PostViewModel[] Posts { get; set; }
   }
}
