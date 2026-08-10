using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ProForma.Shared.Plugins;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace ProForma.Common.Plugins;

public class JsProviderPlugin(ILogger<JsProviderPlugin> _logger) : IPlugin
{
    private Manifest? _manifest;
    private string _entryFileJs = string.Empty;

    public void ConfigurePlugin(IServiceProvider serviceProvider, ref ServiceCollection serviceCollection)
    {
        _logger?.LogInformation($"{nameof(JsProviderPlugin)}.{nameof(ConfigurePlugin)} called.");
    }

    public void SetManifest(Manifest manifest)
    {
        _logger?.LogInformation($"{nameof(JsProviderPlugin)}.{nameof(SetManifest)} called.");
        this._manifest = manifest;
    }
    public void SetFile(string entryFilePath)
    {
        _logger?.LogInformation($"{nameof(JsProviderPlugin)}.{nameof(SetFile)} called.");
        this._entryFileJs = entryFilePath;
    }

    public static Manifest GetManifest()
    {
        return new Manifest()
        {
            Id = "proforma-js-provider-plugin-j96vfssb",
            Name = "ProForma JS Profider Plugin",
            Version = "0.0.1",
            Type = "cs",
            EntryFile = "",            
            Author = "Marlene Pielicke",
            Description = "Plugin for providing js plugins.",
            Homepage = "https://github.com/EbbesTech/ProForma.net",
            Git = "https://github.com/EbbesTech/ProForma.net",
        };
    }
}
