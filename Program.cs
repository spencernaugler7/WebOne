using DotNetEnv;
using Fluid;
using Microsoft.AspNetCore.Mvc;
using WebOne.Models;
using WebOne.Templates;

namespace WebOne;

public partial class Program
{
    private static void Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<Context>();
        builder.Services.AddSingleton<FluidParser>();
        builder.Services.AddTemplateRegistry();
        var app = builder.Build();

        app.MapStaticAssets();

        app.MapGet("/", (context) =>
        {
            context.Response.Redirect("/contacts");
            return Task.CompletedTask;
        });

        app.MapGet("/contacts", async ([FromQuery(Name = "q")] string? query, [FromServices] TemplateRegistry registry, [FromServices] Context context) =>
        {
            List<Contact> contacts = [];
            if (string.IsNullOrEmpty(query))
            {
                contacts = context.Contacts.ToList();
            }
            else
            {
                contacts = context.Contacts
                    .Where(c => !string.IsNullOrEmpty(c.Name) && query.Trim().Contains(c.Name.ToUpper().Trim(), StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
            }

            var html = await registry.RenderTemplateAsync("contacts.liquid", new { Contacts = contacts });
            return Results.Content(html, "text/html");
        });

        app.Run();
    }
}