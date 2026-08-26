using System;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

public class AddAbout
{
  public class Command : IRequest<Result<Unit>>
  {
    public required string Bio { get; set; }
    public string? DisplayName { get; set; }
  }


  public class Handler(IUserAccessor userAccessor, AppDbContext dbContext) : IRequestHandler<Command, Result<Unit>>
  {
    public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
    {
      var user = await userAccessor.GetUserAsync();
      user.Bio = request.Bio;
      user.DisplayName = request.DisplayName?? user.DisplayName;
      var result = await dbContext.SaveChangesAsync() > 0;
      return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem saving changes to DB", 400);
    }
  }
}
