using System;
using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext (DbContextOptions options) : IdentityDbContext<User>(options)
{
    public required DbSet<Activity> Acticities {get;set;}
    public required DbSet<ActivityAttendee> ActivityAttendees {get;set;}
    public required DbSet<Photo> Photos {get;set;}
    public required DbSet<Comments> Comments {get;set;}
    public required DbSet<UserFollowing> UserFollowings {get;set;}

    protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder.Entity<ActivityAttendee>(x => x.HasKey(a => new { a.ActivityId, a.UserId }));

    builder.Entity<ActivityAttendee>()
        .HasOne(x => x.User)
        .WithMany(x => x.Activities)
        .HasForeignKey(x => x.UserId);

    builder.Entity<ActivityAttendee>()
        .HasOne(x => x.Activity)
        .WithMany(x => x.Attendees)
        .HasForeignKey(x => x.ActivityId);

        builder.Entity<UserFollowing>(x =>
        {
            x.HasKey(k => new
            {
                k.ObserverId, k.TargedId
            });
            x.HasOne(o=>o.Observer)
            .WithMany(x=>x.Followings)
            .HasForeignKey(o=> o.ObserverId)
            .OnDelete(DeleteBehavior.NoAction);


            x.HasOne(o=>o.Targed)
            .WithMany(x=>x.Followers)
            .HasForeignKey(o=> o.TargedId)
            .OnDelete(DeleteBehavior.Cascade);        
            });
}
}
