using System.Dynamic;

namespace ProForma.Shared.Plugins;

public interface IPluginInstanceFactory
{
    public void SetPluginFactory(Func<PluginConfigurationItem, IServiceCollection, IPlugin?> factoryAction);
    IPlugin? CreateInstance(PluginConfigurationItem config, IServiceCollection services);
}
