using System.Net;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using WebOneCore;
using static Heimdall.Server.Rendering.FluentHtml;

namespace WebOneWeb.UI;

public static class Error
{
	public static IHtmlContent? ErrorPage(bool showHomeLink, string endpoint, string message) =>
		FluentHtml.Div(div => 
		{
			div.Id("workingContainer");
			div.Class("container-fluid", "overflow-auto");
			div.H1(h1 => h1.Text("Error"));

			if (showHomeLink) 
			{
				div.H3(h3 => h3.A(a => {
					a.Href("/contacts");
					a.Text("go home");
				}));
			}

			if(!string.IsNullOrEmpty(endpoint))
			{
				div.H2(h2 => h2.Text($"EndPoint: {endpoint}"));
			}

			div.Text("Exception Was thrown");
			div.TextArea(text => text.Text(message));

		});
}