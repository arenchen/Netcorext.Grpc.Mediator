using Google.Protobuf;
using Grpc.Core;
using Netcorext.Grpc.Mediator.Abstractions;
using Netcorext.Grpc.Mediator.Pipelines;

namespace Netcorext.Grpc.Mediator;

public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IPipeline> _pipelines;

    public Dispatcher(IServiceProvider serviceProvider, IEnumerable<IPipeline> pipelines)
    {
        _serviceProvider = serviceProvider;
        _pipelines = pipelines;
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        return await SendAsync<TRequest, TResponse>(request, default, cancellationToken);
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, ServerCallContext? context, CancellationToken cancellationToken = default)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        PipelineDelegate pipeline = async (message, ctx, ct) =>
                                    {
                                        var handler = _serviceProvider.GetService(typeof(IMessageHandler<TRequest, TResponse>)) as IMessageHandler<TRequest, TResponse>;

                                        if (handler == null)
                                            throw new ArgumentNullException(nameof(handler));

                                        return await handler.Handle(request, ctx, cancellationToken);
                                    };

        var result = _pipelines.Select(t => new Func<PipelineDelegate, PipelineDelegate>(pipe => async (msg, ctx, token) => await t.InvokeAsync(msg, ctx, pipe, token)))
                               .Reverse()
                               .Aggregate(pipeline, (current, next) => next(current));

        return (TResponse)await result.Invoke(request, context, cancellationToken);
    }
}