using Google.Protobuf;
using Grpc.Core;

namespace Netcorext.Grpc.Mediator.Pipelines;

public interface IPipeline
{
    Task<IMessage> InvokeAsync(IMessage message, ServerCallContext? context, PipelineDelegate next, CancellationToken cancellationToken = default);
}