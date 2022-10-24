using BlogProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BlogProject.ViewModels.Response
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
        public Tag[] Tags { get; set; }
        public CommentViewModel[] Comments { get; set; }
    }
}
