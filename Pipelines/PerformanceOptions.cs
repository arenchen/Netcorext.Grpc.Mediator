namespace Netcorext.Grpc.Mediator.Pipelines;

public class PerformanceOptions
{
    public int SlowCommandTimes { get; set; } = 1000 * 3;
}