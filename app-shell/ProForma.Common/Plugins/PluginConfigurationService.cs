using System.Data;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProForma.Shared.Plugins;

namespace ProForma.Common.Plugins;

public class PluginConfigurationService : IPluginConfigurationService
{
    private readonly ILogger _logger;
    private readonly string _configurationFilename;
    private PluginConfiguration _configuration = new();

    public PluginConfigurationService(ILogger<PluginConfigurationService> logger, [FromKeyedServices("plugin-settings-file")] string configurationFilename)
    {
        _logger = logger;
        _configurationFilename = configurationFilename;
        Load();
    }
    
    public void AddIndexUrl(string newIndexUrl)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(AddIndexUrl)}.");
        if (_configuration.PluginIndexUrls.Contains(newIndexUrl))
            throw new DuplicateNameException($"There exists a index url with the value '{newIndexUrl}'.");
        _configuration.PluginIndexUrls.Add(newIndexUrl);
    }

    public void AddScanFolder(string newScanFolder)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(AddScanFolder)}.");
        if (_configuration.ScanFolders.Contains(newScanFolder))
            throw new DuplicateNameException($"There exists a scan folder path to '{newScanFolder}'.");
        if (!Directory.Exists(newScanFolder))
            throw new DirectoryNotFoundException($"Could not find scan folder '{newScanFolder}'.");
        _configuration.ScanFolders.Add(newScanFolder);
    }

    public void Deinstall(string id)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(Deinstall)}.");
        var item = GetById(id);
        if (item == null) throw new KeyNotFoundException($"Could not find a plugin with id '{id}'");
        _configuration.InstalledPlugins.Remove(item);
    }

    public IEnumerable<PluginConfigurationItem> GetAll()
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(GetAll)}.");
        return _configuration.InstalledPlugins;
    }

    public PluginConfigurationItem? GetById(string id)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(GetById)}.");
        return _configuration.InstalledPlugins.FirstOrDefault(ip => ip.Id == id);
    }

    public IEnumerable<string> GetIndexUrls()
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(GetIndexUrls)}.");
        return _configuration.PluginIndexUrls;
    }

    public IEnumerable<string> GetScanFolders()
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(GetScanFolders)}.");
        return _configuration.ScanFolders;
    }

    public void Install(PluginConfigurationItem newPlugin)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(Install)}.");
        if (_configuration.InstalledPlugins.Any(ip => ip.Id!.Equals(newPlugin.Id)))
            throw new DuplicateNameException("There is a plugin with the same id 'newPlugin.Id'.");
        _configuration.InstalledPlugins.Add(newPlugin);
    }

    public void Load()
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(Load)}.");
        if (!File.Exists(_configurationFilename))
            throw new FileNotFoundException($"Configuration file '{_configurationFilename}' not found.");
        var fileContent = File.ReadAllText(_configurationFilename);
        _configuration = JsonSerializer.Deserialize<PluginConfiguration>(fileContent) ?? throw new SerializationException($"The content of the file '{_configurationFilename}' serialized to null, content: '{fileContent}'");
    }

    public void RemoveIndexUrl(string removeIndexUrl)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(RemoveIndexUrl)}.");
        if (!_configuration.PluginIndexUrls.Contains(removeIndexUrl))
            throw new KeyNotFoundException($"No index url '{removeIndexUrl}' known.");
        _configuration.PluginIndexUrls.Remove(removeIndexUrl);
    }

    public void RemoveScanFolder(string removeScanFolder)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(RemoveScanFolder)}.");
        if (!_configuration.ScanFolders.Contains(removeScanFolder))
            throw new KeyNotFoundException($"No path '{removeScanFolder}' known.");
        _configuration.ScanFolders.Remove(removeScanFolder);
    }

    public void Save()
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(Save)}.");
        var jsonString = JsonSerializer.Serialize(_configuration);
        File.WriteAllText(_configurationFilename, jsonString);
    }

    public void Update(PluginConfigurationItem changedPlugin)
    {
        _logger.LogDebug($"Call {nameof(PluginConfigurationService)}.{nameof(Update)}.");
        var idx = _configuration.InstalledPlugins.FindIndex(0, ip => ip.Id!.Equals(changedPlugin.Id));
        if (idx == -1)
            throw new KeyNotFoundException($"No element with the id '{changedPlugin.Id}' found.");
        _configuration.InstalledPlugins[idx] = changedPlugin;
    }
}