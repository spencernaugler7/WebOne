using Fluid;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Throw;

namespace WebOne.Templates;

public static class TemplateRegistryExtensions
{
    public static IServiceCollection AddTemplateRegistry(this IServiceCollection services)
    {
        services.AddSingleton<FluidParser>();
        services.AddSingleton<IFileProvider>(_ =>
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dir = Path.Combine(currentDir, "Templates");
            var provider = new PhysicalFileProvider(dir, ExclusionFilters.None); // seems brittle, what if we have templates in subdirectories?
            provider.Watch("*.*");
            return provider;
        });

        // what if we want to customize our template options?
        services.AddSingleton<TemplateOptions>((provider) =>
        {
            var fileProvider = provider.GetService<IFileProvider>();
            var options = TemplateOptions.Default;
            options.FileProvider = fileProvider;
            // very unsafe, create some way to register model classes. Perhaps generate from EF core classes?
            options.MemberAccessStrategy = new UnsafeMemberAccessStrategy();
            return options;
        });

        services.AddSingleton<TemplateRegistry>();

        return services;
    }
}

public interface ITemplateRegistry
{
    public ValueTask<string> RenderTemplateAsync(string templateName, object model);
}

public class TemplateRegistry(FluidParser parser, IFileProvider provider, TemplateOptions defaultTemplateOptions): ITemplateRegistry
{
    public async ValueTask<string> RenderTemplateAsync(string templateName, object model)
    {
        var fileInfo = provider.GetFileInfo(templateName);

        fileInfo.ThrowIfNull($"No file found with name: {templateName}");
        fileInfo.PhysicalPath.ThrowIfNull($"File: {templateName} is not directly accessible");

        var text = await File.ReadAllTextAsync(fileInfo.PhysicalPath);
        parser.TryParse(text, out var template)
            .Throw($"Could not parse template {templateName}")
            .IfFalse();

        var context = new TemplateContext(model, defaultTemplateOptions);
        var result = await template.RenderAsync(context);

        return result;
    }
}
