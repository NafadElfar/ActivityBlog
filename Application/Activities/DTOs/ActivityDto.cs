using System;
using Application.Profiles.DTOs;

namespace Application.Activities.DTOs;

public class ActivityDto : BaseActivityDto
{
  public required string Id { get; set; } 
  public bool IsCancelled { get; set; }
  public required string  HostDisplayName { get; set; }
  public required string HostId { get; set; }
  public ICollection<UserProfile> Attendees { get; set; } = [];
}
