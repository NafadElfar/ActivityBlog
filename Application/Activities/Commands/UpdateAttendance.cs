using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class UpdateAttendance
{
  public class Command : IRequest<Result<Unit>>
  {
    public required string Id { get; set; }
  }

  public class Handle(IUserAccessor userAccessor, AppDbContext dbContext) : IRequestHandler<Command, Result<Unit>>
  {
    async Task<Result<Unit>> IRequestHandler<Command, Result<Unit>>.Handle(Command request, CancellationToken cancellationToken)
    {
      var activity = await dbContext.Acticities.Include(x=> x.Attendees).ThenInclude(x=> x.User).SingleOrDefaultAsync(x=>x.Id == request.Id, cancellationToken);
      if(activity == null) return Result<Unit>.Failure("Activity Not found",404);

      var user = await userAccessor.GetUserAsync();

      var attendece = activity.Attendees.FirstOrDefault(x=> x.UserId == user.Id);

      var isHost = activity.Attendees.Any(x=> x.IsHost && x.UserId == user.Id);

      if(attendece != null)
      {
        if(isHost) activity.IsCancelled = !activity.IsCancelled;
        else activity.Attendees.Remove(attendece);
      }
      else
      {
        activity.Attendees.Add(new ActivityAttendee
        {
          UserId = user.Id,
          ActivityId = activity.Id,
          IsHost = false
        });
      }

      var result = await dbContext.SaveChangesAsync(cancellationToken) > 0 ;
      return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("problem updating the DB",400);
    }
  }
}
