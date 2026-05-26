using System.Runtime.CompilerServices;
using System.Text.Json;
using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Throw;
using WebOne;

namespace WebOne.Templates;

public static class TemplateRegistryExtensions
{
    public static IServiceCollection AddTemplateRegistry(this IServiceCollection services)
    {
        services.AddSingleton<FluidParser>();
        services.AddSingleton<IFileProvider>((services) =>
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dir = Path.Combine(currentDir, "Templates");
            var provider = new PhysicalFileProvider(dir, ExclusionFilters.None); // seems brittle, what if we have templates in subdirectories?
            provider.Watch("*.*");
            return provider;
        });

        // what if we want to customize our template options?
        services.AddSingleton<TemplateOptions>((services) =>
        {
            var provider = services.GetService<IFileProvider>();
            var options = TemplateOptions.Default;
            options.FileProvider = provider;
            // very unsafe, create some way to register model classes. Perhaps generate from EF core classes?
            options.MemberAccessStrategy = new UnsafeMemberAccessStrategy();
            return options;
        });

        services.AddSingleton<TemplateRegistry>();

        return services;
    }
}

public class TemplateRegistry(FluidParser Parser, IFileProvider Provider, TemplateOptions DefaultTemplateOptions)
{
    public async ValueTask<string> RenderTemplateAsync(string templateName, object model)
    {
        var fileinfo = Provider.GetFileInfo(templateName);

        fileinfo.ThrowIfNull($"No file found with name: {templateName}");
        fileinfo.PhysicalPath.ThrowIfNull($"File: {templateName} is not directly accessable");

        var text = File.ReadAllText(fileinfo.PhysicalPath);
        Parser.TryParse(text, out IFluidTemplate template)
            .Throw($"Could not parse template {templateName}")
            .IfFalse();

        var context = new TemplateContext(model, DefaultTemplateOptions);
        var result = await template.RenderAsync(context);

        return result;
    }
}
