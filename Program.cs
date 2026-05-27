using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using StarFederation.Datastar.DependencyInjection;
using Throw;
using WebOne.Models;
using WebOne.Templates;

namespace WebOne;

public partial class Program
{
    private static void Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<WebOneDbContext>();
        builder.Services.AddDatastar();
        builder.Services.AddTemplateRegistry();
        builder.Services.AddExceptionHandler<WebOneExceptionHandler>();

        var app = builder.Build();

        app.MapStaticAssets();

        app.MapGet("/", (context) =>
        {
            context.Response.Redirect("/contacts");
            return Task.CompletedTask;
        });

        app.MapGet("/contacts", async ([FromQuery(Name = "q")] string? query, TemplateRegistry registry, WebOneDbContext context) =>
        {
            if (string.IsNullOrEmpty(query))
            {
                var contactsAll = context.Contacts.ToList();
                var emptyContacts = await registry.RenderTemplateAsync("contacts.liquid", new { Contacts = contactsAll });
                return Results.Content(emptyContacts, "text/html");
            }

            List<Contact> contacts = context.Contacts
                .Where(c => !string.IsNullOrEmpty(c.Name) || query.Trim().Contains(c.Name.ToUpper().Trim(), StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            var html = await registry.RenderTemplateAsync("contacts.liquid", new { Contacts = contacts });
            return Results.Content(html, "text/html");
        });

        app.MapGet("/contact/{id}", async (int id, TemplateRegistry registry, WebOneDbContext context, IDatastarService dataStar) =>
        {
            var contact = context.Contacts.FirstOrDefault(c => c.Id == id);
            // Contact? contact = null;
            contact.ThrowIfNull("Contact was in list but entry doesn't exist.");

            var html = await registry.RenderTemplateAsync("contact.liquid", new { Contact = contact });

            await dataStar.PatchElementsAsync(html);
        });

        app.Run();
    }

    private class WebOneExceptionHandler(TemplateRegistry registry, IDatastarService dataStar) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var model = new
            {
                Endpoint = httpContext.Request.GetEncodedUrl(),
                Message = exception.ToString()
            };
            var html = await registry.RenderTemplateAsync("exception.liquid", model);
            await dataStar.PatchElementsAsync(html, cancellationToken);
            return true;
        }
    }
}
