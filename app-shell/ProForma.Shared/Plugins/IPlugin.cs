

namespace ProForma.Shared.Plugins;
public interface IPlugin
{
    void ConfigurePlugin(IServiceProvider serviceProvider, ref ServiceCollection serviceCollection);
    static virtual Manifest GetManifest() => throw new NotImplementedException();
}