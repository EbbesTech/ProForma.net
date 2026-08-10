using System.Text.Json.Serialization;

namespace ProForma.Shared.Plugins;
public class PluginConfiguration
{
    [JsonRequired]
    [JsonPropertyName("scanFolders")]
    public List<string> ScanFolders { get; set; } = [];

    [JsonRequired]
    [JsonPropertyName("pluginIndexUrls")]
    public List<string> PluginIndexUrls { get; set; } = [];

    [JsonRequired]
    [JsonPropertyName("installedPlugins")]
    public List<PluginConfigurationItem> InstalledPlugins { get; set; } = [];
}