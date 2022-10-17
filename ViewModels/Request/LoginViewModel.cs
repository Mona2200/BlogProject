using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels.Request
{
    public class LoginViewModel
    {
      [Required]
      [Display(Name = "Email")]
      [EmailAddress]
      public string Email { get; set; }

      [Required]
      [DataType(DataType.Password)]
      [Display(Name = "Пароль")]
      [StringLength(20, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
      public string Password { get; set; }
   }
}
