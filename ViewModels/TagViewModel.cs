using System.ComponentModel.DataAnnotations;

namespace BlogProject.ViewModels
{
   public class TagViewModel
   {
      [Required]
      [Display(Name = "Название тега")]
      [DataType(DataType.Text)]
      public string Name { get; set; }
   }
}
