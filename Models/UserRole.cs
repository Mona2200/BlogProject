﻿namespace BlogProject.Models
{
   public class UserRole
   {
      public Guid Id { get; set; } = Guid.NewGuid();
      public Guid UserId { get; set; }
      public Guid RoleId { get; set; }
   }
}
