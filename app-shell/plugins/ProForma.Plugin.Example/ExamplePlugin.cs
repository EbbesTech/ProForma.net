using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProForma.Plugin.Example.Services;
using ProForma.Shared.Handlers;
using ProForma.Shared.Plugins;

namespace ProForma.Plugin.Test;

public class ExamplePlugin(ILogger<ExamplePlugin> _logger) : IPlugin
{
    public static Manifest GetManifest()
    {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new DirectoryNotFoundException($"Could not get the path of the '{nameof(ExamplePlugin)}' plugin.");
        var jsonContent = File.ReadAllText(Path.Combine(basePath, "manifest.json"));
        return JsonSerializer.Deserialize<Manifest>(jsonContent) ?? throw new SerializationException($"Could not serialise the manifest content '{jsonContent}'.");
    }

    public void ConfigurePlugin(IServiceProvider serviceProvider, ref ServiceCollection serviceCollection)
    {
        _logger.LogDebug($"Call {nameof(ExamplePlugin)}.{nameof(ConfigurePlugin)}.");
        serviceCollection.AddSingleton<IWindowCustomSchemeHandler, ExampleSchemeHandler>();
        serviceCollection.AddKeyedSingleton<IWindowWebMessageReceivedHandler, ExampleWebMessageReceivedHandler>(nameof(ExampleSchemeHandler));
    }
}