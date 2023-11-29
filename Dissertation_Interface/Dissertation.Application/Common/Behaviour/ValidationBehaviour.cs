using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Shared.Exceptions;

namespace Dissertation.Application.Common.Behaviour;

public class ValidationBehaviour<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehaviour(IValidator<TRequest>? validator = null) => this._validator = validator;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (this._validator is null) return await next();

        ValidationResult? validationResult = await this._validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid) return await next();

        throw new BadRequestException("Invalid Request", validationResult);

        //throw new ValidationException(validationResult.Errors);
    }

}