using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{

  public class Query : IRequest<Result<PageList<ActivityDto,DateTime?>>>
  {
    public required ActivityParams Params { get; set; }
  }

  public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<PageList<ActivityDto,DateTime?>>>
  {
    public async Task<Result<PageList<ActivityDto,DateTime?>>> Handle(Query request, CancellationToken cancellationToken)
    {
      var currentUserId = userAccessor.GetUserId();

      var query = context.Acticities
      .OrderBy(x=>x.Date)
      .Where(x=>x.Date > (request.Params.Cursor ?? request.Params.StartDate))
      .AsQueryable();

      if (!string.IsNullOrEmpty(request.Params.Filter))
      {
        query = request.Params.Filter switch
        {
          "isGoing" => query.Where(x=>x.Attendees.Any(a=>a.UserId == currentUserId && !a.IsHost)),
          "isHost" => query.Where(x=>x.Attendees.Any(a=>a.IsHost && a.UserId == currentUserId)),
          _ => query
        };
      }

      var projectedActivities = query.ProjectTo<ActivityDto>(mapper.ConfigurationProvider, new{CurrentUserId = currentUserId});

      var activities = await projectedActivities
      .Take(request.Params.PageSize + 1)
      .ToListAsync(cancellationToken);

      DateTime ? nextCursor = null;

      if(activities.Count > request.Params.PageSize)
      {
        nextCursor = activities.Last().Date;
        activities.RemoveAt(activities.Count - 1);
      }

      return Result<PageList<ActivityDto,DateTime?>>.Success(
        new PageList<ActivityDto, DateTime?>
        {
          Items = activities,
          NextCursor = nextCursor
        }
      );
    }
  }
}
