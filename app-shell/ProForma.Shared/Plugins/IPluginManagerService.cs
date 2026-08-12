namespace ProForma.Shared.Plugins;

public interface IPluginManagerService
{
    void Scan(IServiceCollection services);
    IPlugin? CreatePluginInstance(PluginConfigurationItem configItem, IServiceCollection services);

    IPlugin? GetPluginById(string id);

    IEnumerable<IPlugin> GetAllPlugins();

    IEnumerable<IPlugin> FilterPlugins(Func<PluginConfigurationItem, bool> filter);

    void LookForUpdates();
    void UpdatePlugin(string id);
}