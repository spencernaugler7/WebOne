using System.Threading.Channels;
using DotNetEnv;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using StarFederation.Datastar.DependencyInjection;
using Throw;
using WebOneCore;
using WebOneWeb.Templates;
using WebOneWeb.UI;

namespace WebOneWeb;

public static class Program
{
    private static Channel<KeyValuePair<string, string>> Events = Channel.CreateUnbounded<KeyValuePair<string, string>>();
    
    private static void Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<WebOneDbContext>();
        builder.Services.AddDatastar();
        builder.Services.AddTemplateRegistry();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<WebOneExceptionHandler>();

        var app = builder.Build();

        app.MapStaticAssets();
        app.UseExceptionHandler();
        app.MapGet("/", (HttpContext context) => context.Response.Redirect("/contacts"));


        app.MapGet("/contacts", async ([FromQuery(Name = "q")] string? query,
           TemplateRegistry registry,
           WebOneDbContext context) =>
        {
            if (!await context.Database.CanConnectAsync())
            {
                var exceptionTemplate = Error.ErrorPage(true, "/contacts", "Database is not connected, Ensure database is running and reconnect.");
                var errorHtml = UiBuilders.Layout(exceptionTemplate);
                return Results.Content(errorHtml?.ToHtmlString(), "text/html");
            }

            if (string.IsNullOrEmpty(query))
            {
                List<Contact> contactsAll = context.Contacts.ToList();
                IHtmlContent? fullContacts = UiBuilders.Layout(UiBuilders.Body(contactsAll));
                return Results.Content(fullContacts.ToHtmlString(), "text/html");
            }

            List<Contact> contacts = context.Contacts
                .Where(c => !string.IsNullOrEmpty(c.Name) || query.Trim().Contains(c.Name.ToUpper().Trim(), StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            var html = UiBuilders.Layout(UiBuilders.Body(contacts));
            return Results.Content(html.ToHtmlString(), "text/html");
        });

        app.Run();
    }
}
