using Microsoft.Extensions.Options;

namespace Proxy
{
    public class ProxyFacade : IProxyFacade
    {
        private readonly HttpClient _monolithClient;
        private readonly HttpClient _moviesClient;
        private readonly IProxyFacade.ProxyOptions _options;

        public ProxyFacade(HttpClient monolithClient, HttpClient moviesClient, IProxyFacade.ProxyOptions options)
        {
            _monolithClient = monolithClient;
            _moviesClient = moviesClient;
            _options = options;
        }

        public async Task<HttpResponseMessage> ResendAsync(string path, HttpRequest httpRequest, CancellationToken cancellationToken)
        {
            var relativeUri = new Uri(path + httpRequest.QueryString, UriKind.Relative);
            var message = CreateProxyRequest(httpRequest, relativeUri);

            if (path.StartsWith("api/movies"))
            {
                if (_options.GradualMigration)
                {
                    var roll = Random.Shared.Next(0, 100);
                    if (roll < _options.MoviesMigrationPercent)
                        return await _moviesClient.SendAsync(message, cancellationToken);

                }
            }
            return await _monolithClient.SendAsync(message, cancellationToken);
        }

        private static HttpRequestMessage CreateProxyRequest(HttpRequest sourceRequest, Uri relativeUri)
        {
            var targetRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(sourceRequest.Method),
                RequestUri = relativeUri
            };

            if (CanHaveBody(sourceRequest.Method))
            {
                targetRequest.Content = new StreamContent(sourceRequest.Body);
            }

            foreach (var header in sourceRequest.Headers)
            {
                if (!targetRequest.Headers.TryAddWithoutValidation(
                        header.Key,
                        header.Value.ToArray()))
                {
                    targetRequest.Content?.Headers.TryAddWithoutValidation(
                        header.Key,
                        header.Value.ToArray());
                }
            }

            return targetRequest;
        }

        private static bool CanHaveBody(string method) =>
            HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsPatch(method);
    }
}
