using System.ComponentModel.DataAnnotations;

namespace BlogProject.ViewModels
{
   public class AddCommentViewModel
   {
      [Required]
      [DataType(DataType.Text)]
      public string Content { get; set; }
   }
}
