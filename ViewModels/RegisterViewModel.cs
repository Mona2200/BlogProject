using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels
{
   public class RegisterViewModel
   {
      [Required]
      [Display(Name = "Имя")]
      [StringLength(50, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 2)]
      [DataType(DataType.Text)]
      public string FirstName { get; set; }

      [Required]
      [Display(Name = "Фамилия")]
      [StringLength(50, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 2)]
      [DataType(DataType.Text)]
      public string LastName { get; set; }

      [Required]
      [Display(Name = "Пол")]
      [StringLength(20)]
      [DataType(DataType.Text)]
      public string Sex { get; set; }

      [Required]
      [Display(Name = "Email")]
      [EmailAddress]
      public string Email { get; set; }

      [Required]
      [DataType(DataType.Password)]
      [Display(Name = "Пароль")]
      [StringLength(20, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
      public string Password { get; set; }

      [Required]
      [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
      [DataType(DataType.Password)]
      [Display(Name = "Подтвердить пароль")]
      public string PasswordConfirm { get; set; }
   }
}
