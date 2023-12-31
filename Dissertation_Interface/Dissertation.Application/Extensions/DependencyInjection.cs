using System.Reflection;
using Dissertation.Application.Common.Behaviour;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dissertation.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

}