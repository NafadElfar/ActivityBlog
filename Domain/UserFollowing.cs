using System;

namespace Domain;

public class UserFollowing
{
  public required string ObserverId { get; set; }
  public  User Observer { get; set; } = null!;
  public required string TargedId { get; set; }
  public  User Targed { get; set; } = null!;
}
