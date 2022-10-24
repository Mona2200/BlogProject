using BlogProject.Models;

namespace BlogProject.ViewModels.Response
{
    public class CommentViewModel
    {
      public string Content { get; set; }
      public Post Post { get; set; }
      public User User { get; set; }
   }
}
