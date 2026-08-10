using System.Text.Json.Serialization;

namespace ProForma.Shared.Plugins;

public class PluginConfigurationItem : Manifest
{
    [JsonRequired]
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = false;

    [JsonRequired]
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonIgnore]
    public IPlugin? PluginInstance{ get; set; } = null;
}