using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using WebOneCore;
using static Heimdall.Server.Rendering.FluentHtml;

namespace WebOneWeb.UI;

public static class UiBuilders
{

    public static IHtmlContent? Layout(IHtmlContent? body) => HtmlTag(html =>
    {
        html.Head(head =>
        {
            head.Meta(m => m.Attr("charset", "UTF-8"));
            head.Meta(m => m.Attr("charset", "UTF-8"));
            head.Title(t => t.Text("Hello World"));
            head.Meta(meta => meta.Attrs("name", "color-scheme", "content", "light dark"));
            head.Meta(meta => meta.Attrs("name", "viewport", "content", "width=device-width,initial-scale=1"));
            // styles
            head.Link(link => link.Attrs("rel", "stylesheet", "href", "/css/custom-styles.css"));
            head.Link(link => link.Attrs("rel", "stylesheet", "href", "/lib/pico/pico.min.css"));
            // scripts
            head.Script(script => script.Attrs("type", "module", "src", "/js/datastar.min.js"));
        });
        html.Pre(p => p.Attr("data-json-signals", null));
        html.Body(b => b.Main(main => 
        {
            main.Class("container-fluid overflow-auto");
            main.Add(body);
        }));
    });

    public static IHtmlContent? Body(List<Contact> contacts) => Div(div =>
    {
        div.Id("workingContainer");
        div.Data("theme", "dark");

        div.Table(t => t.TableBody(tbody => 
           tbody.ForEach(contacts, (builder, contact) => builder.Add(ContactRow(contact)))));
          
    });

    public static IHtmlContent? ContactRow(Contact contact) => TableRow(trow =>
    {
        trow.Id(contact.Id.ToString());
        trow.Data("on-click", $"@get('/contact/{contact.Id})");

        trow.TableHead(head => head.Img(img => img.Attrs("src", "/images/usr.svg", "width", "50", "height", "50")));
        trow.TableHead(head => head.Div(d =>
        {
            d.Id("contactDescription");
            d.Class("contact-description");
            d.Text($"{contact.Name} - {contact.Email}");
        }));
    });
}

public static class HtmlExtensions
{
    public static ElementBuilder Attrs(this ElementBuilder builder, params string[] attrs)
    {
        if (attrs.Length % 2 != 0)
            throw new ArgumentException("Attrs must be provided an even number of key/values");

        foreach (var keyval in attrs.Chunk(2))
        {
            builder.Attr(keyval[0], keyval[^1]);
        }

        return builder;
    }

    public static ElementBuilder ForEach<T>(this ElementBuilder builder, 
        IEnumerable<T> collection, 
        Action<ElementBuilder, T> individualElementBuilder)  
    {
        foreach(var item in collection) 
        {
            individualElementBuilder(builder, item);
        }
        return builder;
    }

    public static async Task<ElementBuilder> ForEachAsync<T>(this ElementBuilder builder, 
        IAsyncEnumerable<T> collection, 
        Func<ElementBuilder, T, Task> individualElementBuilder)  
    {
        await foreach(var item in collection) 
        {
            await individualElementBuilder(builder, item);
        }
        return builder;
    }

    public static async Task<ElementBuilder> ForEachAsync<T>(this ElementBuilder builder, 
        IEnumerable<T> collection, 
        Func<ElementBuilder, T, Task> individualElementBuilder)  
    {
        foreach(var item in collection) 
        {
            await individualElementBuilder(builder, item);
        }
        return builder;
    }
}
