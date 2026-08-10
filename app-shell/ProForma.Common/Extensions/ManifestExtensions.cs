namespace ProForma.Shared.Plugins.Extensions;

public static class ManifestExtensions
{
    extension(Manifest manifest)
    {
        public PluginConfigurationItem ToPluginConfigurationItem()
        {
            return new PluginConfigurationItem
            {
                Id = manifest.Id,
                Name = manifest.Name,
                Version = manifest.Version,
                Type = manifest.Type,
                EntryFile = manifest.EntryFile,
                Description = manifest.Description,
                Author = manifest.Author,
                Git = manifest.Git,
                Homepage = manifest.Homepage,
                Path = string.Empty,
                IsEnabled = false
            };
        }
    }

    extension(PluginConfigurationItem pluginConfigurationItem)
    {
        public Manifest ToManifest()
        {
            return new Manifest
            {
                Id = pluginConfigurationItem.Id,
                Name = pluginConfigurationItem.Name,
                Version = pluginConfigurationItem.Version,
                Type = pluginConfigurationItem.Type,
                EntryFile = pluginConfigurationItem.EntryFile,
                Description = pluginConfigurationItem.Description,
                Author = pluginConfigurationItem.Author,
                Git = pluginConfigurationItem.Git,
                Homepage = pluginConfigurationItem.Homepage,
            };
        }
    }
}
