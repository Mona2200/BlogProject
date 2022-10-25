using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels.Request
{
   public class AddPostViewModel
   {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Заголовок поста")]
      [DataType(DataType.Text)]
      [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
      public string Title { get; set; }
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Содержание поста")]
      [DataType(DataType.Text)]
      [StringLength(20000, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 5)]
      public string Content { get; set; }
   }
}
