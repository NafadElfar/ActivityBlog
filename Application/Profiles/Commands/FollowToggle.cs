using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Commands;

public class FollowToggle
{
  public class Command : IRequest<Result<Unit>>
  {
    public required string TargedUserId { get; set; }
  }


  public class Handler(AppDbContext dbContext, IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
  {
    public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
    {
      var observer = await userAccessor.GetUserAsync();

      var targed = await dbContext.Users.FirstOrDefaultAsync(
          user => user.Id == request.TargedUserId,
          cancellationToken);

      if (targed == null)
          return Result<Unit>.Failure("User not found", 404);

      var Following = await dbContext.UserFollowings
          .FindAsync([observer.Id, targed.Id], cancellationToken);

      if(Following == null) dbContext.UserFollowings.Add(new UserFollowing
      {
        ObserverId = observer.Id,
        TargedId = request.TargedUserId,
      });
      else dbContext.UserFollowings.Remove(Following);
      
      var result = await dbContext.SaveChangesAsync() > 0 ;

      return result ?
      Result<Unit>.Success(Unit.Value) 
      : Result<Unit>.Failure("Problem updating following",400);
    }
  }
}
