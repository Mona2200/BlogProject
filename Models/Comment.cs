namespace BlogProject.Models
{
   public class Comment
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public string Content { get; set; }
      public Guid PostId { get; set; }
      public Guid UserId { get; set; }
   }
}
