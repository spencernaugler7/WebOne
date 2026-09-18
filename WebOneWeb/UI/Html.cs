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

        trow.TableDataCell(cell => cell.Img(img => img.Attrs("src", "/images/usr.svg", "width", "50", "height", "50")));
        trow.TableDataCell(cell => cell.Div(div =>
        {
            div.Id("contactDescription");
            div.Class("contact-description");
            div.Text($"{contact.Name} - {contact.Email}");
        }));
    });
}
