using System;
using Application.Core;
using Application.Activities.DTOs;
using MediatR;
using Persistence;
using AutoMapper;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace Application.Activities.Commands;

public class AddCommnet
{
  public class Command : IRequest<Result<CommentDto>>
  {
    public required string Body { get; set; }
    public required string ActivityId { get; set; }
  }

  public class Handler(AppDbContext dbContext , IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Command, Result<CommentDto>>
  {
    public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
    {
      var activity = await dbContext.Acticities
      .Include(x=>x.Comments)
      .ThenInclude(x=>x.User)
      .FirstOrDefaultAsync(x=>x.Id == request.ActivityId, cancellationToken);

      if(activity == null) return Result<CommentDto>.Failure("Activity not found",404);

      var user = await userAccessor.GetUserAsync();

      var comment = new Comments
      {
        Body = request.Body,
        UserId = user.Id,
        ActivityId = request.ActivityId
      };

      activity.Comments.Add(comment);

      var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

      return result ? Result<CommentDto>.Success(mapper.Map<CommentDto>(comment)) : Result<CommentDto>.Failure("Faild to add the comment",400);
    }
  }
}
