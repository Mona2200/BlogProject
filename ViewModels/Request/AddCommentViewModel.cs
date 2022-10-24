﻿using BlogProject.ViewModels.Response;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.ViewModels.Request
{
    public class AddCommentViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Content { get; set; }
      public PostViewModel Post { get; set; }
   }
}
