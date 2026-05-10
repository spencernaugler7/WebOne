using System.Diagnostics;
using Fluid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace WebOne;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services
            .AddSingleton<FluidParser>()
            .AddSingleton<PhysicalFileProvider>((services) =>
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
                var provider = new PhysicalFileProvider(dir, ExclusionFilters.None);
                provider.Watch("*.*");
                return provider;
            })
            .AddSingleton<TemplateOptions>((services) =>
            {
                var provider = services.GetService<PhysicalFileProvider>();
                var options = TemplateOptions.Default;
                options.FileProvider = provider;
                return options;
            })
            .AddKeyedScoped<IFluidTemplate>("index", (IServiceProvider services, object key) =>
            {
                var provider = services.GetService<PhysicalFileProvider>();
                var parser = services.GetService<FluidParser>();

                var fileinfo = provider.GetFileInfo("index.liquid");
                var text = File.ReadAllText(fileinfo?.PhysicalPath ?? string.Empty);

                if (!parser.TryParse(text, out IFluidTemplate template))
                {
                    throw new Exception("Could not parse");
                }
                return template;
            });

        var app = builder.Build();
        app.MapStaticAssets();

        app.MapGet("/", (context) =>
        {
            context.Response.Redirect("/html");
            return Task.CompletedTask;
        });

        app.MapGet("/html", async ([FromQuery(Name = "q")] string? query, [FromKeyedServices("index")] IFluidTemplate template, TemplateOptions options) =>
        {
            List<string> contacts = [];
            if (query is null)
                contacts = FakeDb.Contacts;
            else
                contacts = FakeDb.Contacts
                    .Where(contact => contact.ToUpper().Trim().Contains(query.ToString().ToUpper().Trim()))
                    .ToList();

            var context = new TemplateContext(new { Contacts = contacts }, options);
            var final = await template.RenderAsync(context);

            return Results.Content(final, "text/html");
        });

        app.Run();
    }
}

public static class FakeDb
{
    public static List<string> Contacts { get; set; } = ["Ben", "James"];
}
