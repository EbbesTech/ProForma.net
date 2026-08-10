using System.Net.Mime;
using System.Reflection;
using System.Reflection.Metadata;

namespace ProForma.Shared.Handlers;

public interface IWindowCustomSchemeHandler
{
    string GetScheme();
    bool CanHandle(object? sender, string scheme);
    MemoryStream Handle(object? sender, string scheme, string url, out string contentType);
}
