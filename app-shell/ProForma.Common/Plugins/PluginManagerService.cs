using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProForma.Shared.Plugins;
using ProForma.Shared.Plugins.Extensions;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProForma.Common.Plugins;

public class PluginManagerService(
    ILogger<PluginManagerService> _logger, 
    IServiceProvider _serviceProvider, 
    IPluginConfigurationService _pluginConfigurations, 
    JsonSerializerOptions _jsonSerializerOptions) : IPluginManagerService
{
    public IEnumerable<IPlugin> GetAllPlugins()
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(GetAllPlugins)}.");
        return _pluginConfigurations.GetAll()
            .Where(plugin => plugin.PluginInstance is not null)
            .Select(plugin => plugin.PluginInstance) as IEnumerable<IPlugin> ?? [];
    }

    public IPlugin GetPluginById(string id)
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(GetPluginById)}.");
        return _pluginConfigurations.GetById(id)?.PluginInstance ?? throw new KeyNotFoundException($"Plugin with id '{id}' not found.");
    }

    public void LookForUpdates()
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(LookForUpdates)}.");
        throw new NotImplementedException();
        // ToDo: Implement logic to check for plugin updates, possibly by comparing version numbers or checking a remote repository.
    }

    public void Scan(IServiceCollection services)
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(Scan)}.");
        var scanFolders = _pluginConfigurations.GetScanFolders() as List<string>;
        scanFolders?.SelectMany(sf =>
        {
            if (sf.StartsWith('.'))
            {
                sf = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sf));
            }
            if (!Directory.Exists(sf))
                throw new DirectoryNotFoundException($"Directory '{sf}' not found.");
            return Directory.EnumerateDirectories(sf, "*", SearchOption.AllDirectories)
                .Where(dir =>
                    File.Exists(Path.Combine(dir, "manifest.json"))
                    && (
                        File.Exists(Path.Combine(dir, "plugin.js"))
                        || Directory.EnumerateFiles(dir, "*.dll").Any()
                        )
                );
        })?.Select(pluginFolder =>
        {
            var manifest = GetManifestFromJson(pluginFolder);
            var pluginType = PluginManagerService.ValidateType(manifest.Type);
            var configItem = manifest.ToPluginConfigurationItem();
            configItem.Path = pluginFolder;
            configItem.IsEnabled = true;
            configItem.PluginInstance = CreatePluginInstance(configItem, services);
            return configItem;
        })?.ToList().ForEach(plugin =>
        {
            if (_pluginConfigurations.GetById(plugin.Id!) is null)
                _pluginConfigurations.Install(plugin);
            else
                _pluginConfigurations.Update(plugin);
            _pluginConfigurations.Save();
        });
    }

    private static string ValidateType(string? type)
    {
        if (!string.IsNullOrEmpty(type)
            && (type.Equals("js") || type.Equals("cs")))
        {
            return type;
        }
        throw new NotSupportedException($"Plugin type '{type}' is not supported.");
    }

    public void UpdatePlugin(string id)
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(UpdatePlugin)}.");
        throw new NotImplementedException();
    }

    public Manifest GetManifestFromAssembly(IPlugin plugin) 
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(GetManifestFromAssembly)}.");
        return plugin.GetType().GetMethod("GetManifest", System.Reflection.BindingFlags.Static)?.Invoke(plugin, null) is Manifest manifest
            ? manifest
            : throw new InvalidOperationException($"Plugin '{plugin.GetType().FullName}' does not implement GetManifest() correctly.");
    }
    public Manifest GetManifestFromJson(string folderPath)
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(GetManifestFromJson)}.");
        if (!File.Exists(Path.Combine(folderPath, "manifest.json")))
            throw new FileNotFoundException($"Manifest file not found in '{folderPath}'.");
        var jsonContent = File.ReadAllText(Path.Combine(folderPath, "manifest.json"));
        return JsonSerializer.Deserialize<Manifest>(jsonContent, _jsonSerializerOptions) ?? throw new InvalidOperationException($"Failed to deserialize manifest.json in '{folderPath}'.");
    }

    public IPlugin? CreatePluginInstance(PluginConfigurationItem configItem, IServiceCollection serviceCollection)
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(CreatePluginInstance)}.");
        var createInstanceService = _serviceProvider.GetRequiredKeyedService<IPluginInstanceFactory>(configItem.Type);
        if (createInstanceService is not null)
            return createInstanceService.CreateInstance(configItem, serviceCollection);
        return null;
    }

    public IEnumerable<IPlugin> FilterPlugins(Func<PluginConfigurationItem, bool> filter)
    {
        _logger.LogDebug($"Call {nameof(PluginManagerService)}.{nameof(FilterPlugins)}.");            
        return _pluginConfigurations.GetAll()
            .Where(filter)
            .Select(plugin => plugin.PluginInstance) as IEnumerable<IPlugin> ?? [];
    }
}