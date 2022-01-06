using Google.Protobuf;
using Grpc.Core;

namespace Netcorext.Grpc.Mediator.Abstractions;

public interface IMessageHandler { }

public interface IMessageHandler<in TRequest, TResponse> : IMessageHandler
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>
{
    Task<TResponse> Handle(TRequest request, ServerCallContext? context, CancellationToken cancellationToken = default);
}