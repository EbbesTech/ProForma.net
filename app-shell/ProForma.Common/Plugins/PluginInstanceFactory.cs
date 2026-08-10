using Microsoft.Extensions.DependencyInjection;

using ProForma.Shared.Plugins;

using System;
using System.Collections.Generic;
using System.Text;

namespace ProForma.Common.Plugins;

public class PluginInstanceFactory : IPluginInstanceFactory
{
    private Func<PluginConfigurationItem, IServiceCollection, IPlugin?>? _factoryAction;
    public IPlugin? CreateInstance(PluginConfigurationItem config, IServiceCollection services)
    {
        return _factoryAction?.Invoke(config, services);
    }

    public void SetPluginFactory(Func<PluginConfigurationItem, IServiceCollection, IPlugin?> factoryAction)
    {
        _factoryAction = factoryAction;
    }
}
