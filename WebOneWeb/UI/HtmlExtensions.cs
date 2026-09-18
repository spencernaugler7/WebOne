using static Heimdall.Server.Rendering.FluentHtml;

namespace WebOneWeb.UI;

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
