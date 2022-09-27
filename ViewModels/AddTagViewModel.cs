using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels
{
   public class AddTagViewModel
   {
      [Required]
      [Display(Name = "Название тега")]
      [DataType(DataType.Text)]
      public string Name { get; set; }
   }
}
