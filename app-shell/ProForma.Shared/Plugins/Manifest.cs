using System.Text.Json.Serialization;

namespace ProForma.Shared.Plugins;

public class Manifest
{
    [JsonRequired]
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonRequired]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonRequired]
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonRequired]
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonRequired]
    [JsonPropertyName("entryFile")]
    public string? EntryFile { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("homepage")]
    public string? Homepage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("git")]
    public string? Git { get; set; }
}