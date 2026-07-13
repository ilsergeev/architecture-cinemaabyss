
namespace Proxy
{
    public interface IProxyFacade
    {
        Task<HttpResponseMessage> ResendAsync(string path, HttpRequest httpRequest, CancellationToken cancellationToken);

        public class ProxyOptions
        {
            public bool GradualMigration { get; set; }
            public int MoviesMigrationPercent { get; set; }
        }
    }
}