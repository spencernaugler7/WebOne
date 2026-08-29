using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using StarFederation.Datastar.DependencyInjection;
using Throw;
using WebOne.Templates;

namespace WebOne;

public sealed class WebOneExceptionHandler(TemplateRegistry registry, IServiceProvider provider) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
        await using var scope = provider.CreateAsyncScope();

        var dataStar = scope.ServiceProvider.GetService<IDatastarService>();
        dataStar.ThrowIfNull("The DataStar service cannot be null");

        var html = await registry.RenderTemplateAsync("exception.liquid", new
        {
            ShowHomeLink = true,
            Endpoint = httpContext.Request.GetEncodedUrl(),
            Message = exception.ToString()
        });
        await dataStar.PatchElementsAsync(html, token);
        return true;
    }
}
