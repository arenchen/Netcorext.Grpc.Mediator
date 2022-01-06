using FluentValidation;
using Google.Protobuf;
using Grpc.Core;

namespace Netcorext.Grpc.Mediator.Pipelines;

public class ValidatorPipeline : IPipeline
{
    private readonly IServiceProvider _serviceProvider;

    public ValidatorPipeline(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IMessage> InvokeAsync(IMessage message, ServerCallContext? context, PipelineDelegate next, CancellationToken cancellationToken = default)
    {
        var genericType = typeof(IValidator<>).MakeGenericType(message.GetType());

        if (_serviceProvider.GetService(genericType) is not IValidator validator) goto nextStep;

        var validationContext = new ValidationContext<object>(message);
        var result = await validator.ValidateAsync(validationContext, cancellationToken);

        if (!result.IsValid) throw new ValidationException(result.Errors);

        nextStep:

        return await next(message, context, cancellationToken);
    }
}