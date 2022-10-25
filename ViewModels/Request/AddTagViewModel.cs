using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels.Request
{
    public class AddTagViewModel
    {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Название тега")]
        [DataType(DataType.Text)]
      [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 1)]
        public string Name { get; set; }
    }
}
