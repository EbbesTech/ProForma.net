using System.Text;
using Microsoft.Extensions.Logging;
using ProForma.Shared.Handlers;

namespace ProForma.Plugin.Example.Services;

public class ExampleSchemeHandler(ILogger<ExampleSchemeHandler> _logger) : IWindowCustomSchemeHandler
{
    private static readonly string _scheme = "example";
    public bool CanHandle(object? sender, string scheme)
    {
        _logger.LogDebug($"Call {nameof(ExampleSchemeHandler)}.{CanHandle}");
        return _scheme.Equals(scheme, StringComparison.InvariantCultureIgnoreCase);
    }

    public string GetScheme()
    {
        _logger.LogDebug($"Call {nameof(ExampleSchemeHandler)}.{GetScheme}");
        return _scheme;
    }

    public MemoryStream Handle(object? sender, string scheme, string url, out string contentType)
    {
        _logger.LogDebug($"Call {nameof(ExampleSchemeHandler)}.{Handle}");
        contentType = "text/javascript";
        return new MemoryStream(Encoding.UTF8.GetBytes(@"
            (() =>{
                window.setTimeout(() => {
                    alert(`🎉 Dynamically inserted JavaScript.`);
                }, 1000);
            })();
        "));
    }
}