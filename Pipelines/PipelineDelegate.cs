using Google.Protobuf;
using Grpc.Core;

namespace Netcorext.Grpc.Mediator.Pipelines;

public delegate Task<IMessage> PipelineDelegate(IMessage message, ServerCallContext? context, CancellationToken cancellationToken = default);