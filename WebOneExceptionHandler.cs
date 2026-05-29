using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using StarFederation.Datastar.DependencyInjection;
using Throw;
using WebOne.Templates;

namespace WebOne;

public partial class Program
{
    public sealed class WebOneExceptionHandler(TemplateRegistry registry, IServiceProvider provider) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            using var scope = provider.CreateScope();
            var dataStar = scope.ServiceProvider.GetService<IDatastarService>();
            dataStar.ThrowIfNull("Datastar service cannot be null");

            var model = new
            {
                ShowHomeLink = true,
                Endpoint = httpContext.Request.GetEncodedUrl(),
                Message = exception.ToString()
            };
            var html = await registry.RenderTemplateAsync("exception.liquid", model);
            var options = new PatchElementsOptions();
            await dataStar.PatchElementsAsync(html, cancellationToken);
            return true;
        }
    }
}
