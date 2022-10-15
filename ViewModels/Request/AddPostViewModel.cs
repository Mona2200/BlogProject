using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels.Request
{
    public class AddPostViewModel
    {
        [Required]
        [Display(Name = "Заголовок поста")]
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Содержание поста")]
        [DataType(DataType.Text)]
        public string Content { get; set; }
    }
}
