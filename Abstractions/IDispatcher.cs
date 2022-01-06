using Google.Protobuf;
using Grpc.Core;

namespace Netcorext.Grpc.Mediator.Abstractions;

public interface IDispatcher
{
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;

    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, ServerCallContext? context, CancellationToken cancellationToken = default)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;
}