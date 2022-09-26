using BlogProject.Models;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.ViewModels
{
   public class CommentViewModel
   {
   [Required]
   [DataType(DataType.Text)]
      public string Content { get; set; }
   }
}
