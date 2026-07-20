using Microsoft.AspNetCore.Mvc;

namespace Proxy.Controllers
{
    public class ProxyController : ControllerBase
    {
        //catch-all
        [Route("{**path}")]
        public async Task Proxy(
            [FromServices] IProxyFacade proxyFacade,
            string path,
            CancellationToken cancellationToken = default)
        {
            using var proxyProcessingResult = await proxyFacade.ResendAsync(path, Request, cancellationToken);

            Response.StatusCode = (int)proxyProcessingResult.StatusCode;

            foreach(var header in proxyProcessingResult.Headers)
                Response.Headers[header.Key] = header.Value.ToArray();
            foreach (var header in proxyProcessingResult.Content.Headers)
                Response.Headers[header.Key] = header.Value.ToArray();

            Response.Headers.Remove("transfer-encoding");
            await proxyProcessingResult.Content.CopyToAsync(Response.Body, cancellationToken);
        }
    }
}
