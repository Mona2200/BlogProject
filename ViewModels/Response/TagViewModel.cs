using BlogProject.Models;

namespace BlogProject.ViewModels.Response
{
    public class TagViewModel
    {
    public Tag[] Tags { get; set; }
    public Guid TagId { get; set; }
    public string TagName { get; set; }
    }
}
