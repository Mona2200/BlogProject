using BlogProject.ViewModels.Response;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.ViewModels.Request
{
    public class AddCommentViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [DataType(DataType.Text)]
      [StringLength(255, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 1)]
        public string Content { get; set; }
      public PostViewModel Post { get; set; }
   }
}
