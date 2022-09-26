namespace BlogProject.Models
{
   public class Post
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public string Title { get; set; }
      public string Content { get; set; }
      public Guid UserId { get; set; }
   }
}
