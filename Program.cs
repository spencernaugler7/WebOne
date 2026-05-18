using Fluid;
using Microsoft.AspNetCore.Mvc;
using OneOf.Types;
using WebOne.Templates;

namespace WebOne;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<FluidParser>();
        builder.Services.AddTemplateRegistry();
        var app = builder.Build();

        app.MapStaticAssets();

        app.MapGet("/", (context) =>
        {
            context.Response.Redirect("/contacts");
            return Task.CompletedTask;
        });

        app.MapGet("/contacts", ([FromQuery(Name = "q")] string? query, [FromServices] TemplateRegistry registry) =>
        {
            List<Contact> contacts = [];
            if (query is null)
                contacts = [.. FakeDb.Contacts];
            else
                contacts = FakeDb.Contacts
                    .Where(contact => contact.Name.ToUpper().Trim().Contains(query.ToString().ToUpper().Trim()))
                    .ToList();

            var html = registry.RenderTemplateAsync("contacts.liquid", new { Contacts = contacts });
            return Results.Content(html, "text/html");
        });

        app.Run();
    }
}

public static class FakeDb
{
    public static List<Contact> Contacts { get; set; } = [
        new(){ Name = "Ben", Email = "test@gmail.com" },
        new(){ Name = "James", Email = "test2@gmail.com"}
    ];
}

public class Contact
{
    public required string Name { get; set; }
    public required string Email { get; set; }
}
