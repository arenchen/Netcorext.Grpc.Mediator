using System.Diagnostics;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Netcorext.Grpc.Mediator.Pipelines;

public class PerformancePipeline : IPipeline
{
    private readonly PerformanceOptions _options;
    private readonly ILogger<PerformancePipeline> _logger;

    public PerformancePipeline(PerformanceOptions options, ILogger<PerformancePipeline> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<IMessage> InvokeAsync(IMessage message, ServerCallContext? context, PipelineDelegate next, CancellationToken cancellationToken = default)
    {
        var stopwatch = new Stopwatch();
        var type = message.GetType();

        try
        {
            stopwatch.Start();

            return await next(message, context, cancellationToken);
        }
        finally
        {
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > _options.SlowCommandTimes)
                _logger.LogWarning($"'{type.FullName}' processing too slow, elapsed: {stopwatch.Elapsed}");
        }
    }
}