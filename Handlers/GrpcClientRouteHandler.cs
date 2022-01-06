namespace Netcorext.Grpc.Handlers;

public class GrpcClientRouteHandler : DelegatingHandler
{
    private readonly string _route;

    public GrpcClientRouteHandler(string route) : this(new HttpClientHandler(), route) { }

    public GrpcClientRouteHandler(HttpMessageHandler innerHandler, string route)
        : base(innerHandler)
    {
        _route = route;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = $"{request.RequestUri.Scheme}://{request.RequestUri.Host}";

        url += $"{_route}{request.RequestUri.AbsolutePath}";

        request.RequestUri = new Uri(url, UriKind.Absolute);

        return base.SendAsync(request, cancellationToken);
    }
}