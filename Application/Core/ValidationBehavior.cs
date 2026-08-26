using System;
using FluentValidation;
using MediatR;

namespace Application.Core;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if(validator == null) return await next();

    var validationRequest = await validator.ValidateAsync(request, cancellationToken);

    if (!validationRequest.IsValid)
    {
        throw new ValidationException(validationRequest.Errors);
    }

    return await next();
  }
}
