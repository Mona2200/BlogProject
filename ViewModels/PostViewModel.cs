using BlogProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels
{
   public class PostViewModel
   {
      public Guid Id { get; set; }
      public string Title { get; set; }
      public string Content { get; set; }
      public Tag[] Tags { get; set; }
      public Comment[] Comments { get; set; }
   }
}
