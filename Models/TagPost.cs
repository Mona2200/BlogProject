namespace BlogProject.Models
{
   public class TagPost
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public Guid TagId { get; set; }
      public Guid PostId { get; set; }
   }
}
