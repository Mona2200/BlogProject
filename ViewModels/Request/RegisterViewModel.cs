﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels.Request
{
   public class RegisterViewModel
   {
      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Имя")]
      [StringLength(50, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 2)]
      [DataType(DataType.Text)]
      public string FirstName { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Фамилия")]
      [StringLength(50, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 2)]
      [DataType(DataType.Text)]
      public string LastName { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Пол")]
      [StringLength(20)]
      [DataType(DataType.Text)]
      public string Sex { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Display(Name = "Email")]
      [EmailAddress]
      public string Email { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [DataType(DataType.Password)]
      [Display(Name = "Пароль")]
      [StringLength(20, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 8)]
      public string Password { get; set; }

      [Required(ErrorMessage = "Данное поле обязательно для заполнения")]
      [Compare("Password", ErrorMessage = "Пароли не совпадают")]
      [DataType(DataType.Password)]
      [Display(Name = "Подтвердить пароль")]
      public string PasswordConfirm { get; set; }
   }
}
