using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using WebOneCore;
using static Heimdall.Server.Rendering.FluentHtml;

namespace WebOneWeb.UI;

public static class Html
{
    public static string Layout(IHtmlContent? body)
    {
        var builder = FluentHtml.HtmlTag(html =>
        {
            html.Head(head =>
            {
                head.Meta(m => m.Attr("charset", "UTF-8"));
                head.Meta(m => m.Attr("charset", "UTF-8"));
                head.Title(t => t.Text("Hello World"));
            });
            html.Pre(p => p.Attr("data-json-signals", null));
            html.Body(b =>
            {
                b.Attr("class", "container-fluid");
                b.Add(body);
            });
        });

        return builder.ToHtmlString();
    }

    public static IHtmlContent? Body(List<Contact> contacts)
    {
        var builder = FluentHtml.Div(div =>
        {
            div.Id("workingContainer");
            div.Class("container-fluid", "overflow-auto");
            div.Data("theme", "dark");

            div.Table(t => t.TableBody(tbody =>
            {
                foreach (var contact in contacts)
                {
                    tbody.TableRow(row =>
                    {
                        row.Id(contact.Id.ToString());
                        row.Data("on-click", $"@get('/contact/{contact.Id})");

                        row.TableHead(head => head.Img(img => img.Attrs("src", "/images/usr.svg", "width", "50", "height", "50")));
                        row.TableHead(head =>
                        {
                            head.Id("contactDescription");
                            head.Class("contact-description");
                            head.Text($"{contact.Name} - {contact.Email}");
                        });
                    });
                }
            }));
        });
        return builder;
    }
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
}
