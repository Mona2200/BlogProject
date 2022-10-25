using BlogProject.Models;

namespace BlogProject.ViewModels.Request
{
   public class FormPostViewModel
   {
      public Guid postId { get; set; }
      public AddPostViewModel Post { get; set; }
      public string TagIds { get; set; }
      public Tag[] AllTags { get; set; }
      public AddTagViewModel AddTag { get; set; }
   }
}
