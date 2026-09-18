using Microsoft.AspNetCore.Html;
using WebOneCore;
using static Heimdall.Server.Rendering.FluentHtml;

namespace WebOneWeb.UI;

public static class UiBuilders
{
    public static IHtmlContent? Layout(IHtmlContent? bodyTemplate) => HtmlTag(html =>
        html.Head(head => head.Meta(m => m.Attr("charset", "UTF-8"))
            .Meta(m => m.Attr("charset", "UTF-8"))
            .Title(t => t.Text("Hello World"))
            .Meta(meta => meta.Attrs("name", "color-scheme", "content", "light dark"))
            .Meta(meta => meta.Attrs("name", "viewport", "content", "width=device-width,initial-scale=1"))
            // styles
            .Link(link => link.Attrs("rel", "stylesheet", "href", "/css/custom-styles.css"))
            .Link(link => link.Attrs("rel", "stylesheet", "href", "/lib/pico/pico.min.css"))
            // scripts
            .Script(script => script.Attrs("type", "module", "src", "/js/datastar.min.js")))
        .Pre(p => p.Attr("data-json-signals", null))
        .Body(body => body.Main(main => main
                .Class("container-fluid overflow-auto")
                .Add(bodyTemplate))));

    public static IHtmlContent? Body(List<Contact> contacts) => Div(div => div
        .Id("workingContainer")
        .Data("theme", "dark")
        .Table(t => t.TableBody(tbody => tbody
            .ForEach(contacts, (builder, contact) => builder.Add(ContactRow(contact))))));

    public static IHtmlContent? ContactRow(Contact contact) => TableRow(trow => trow.Id(contact.Id.ToString())
        .Data("on-click", $"@get('/contact/{contact.Id})")
        .TableDataCell(cell => cell.Img(img =>
            img.Attrs("src", "/images/usr.svg", "width", "50", "height", "50")))
        .TableDataCell(cell => cell.Div(div => div.Id("contactDescription")
            .Class("contact-description")
            .Text($"{contact.Name} - {contact.Email}"))));
}
