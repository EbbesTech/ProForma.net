using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Photino.NET;
using Photino.NET.Server;

using ProForma.Common.Extensions;
using ProForma.Common.FileServer;
using ProForma.Common.Guard;
using ProForma.Common.Plugins;
using ProForma.Shared.FileServer;
using ProForma.Shared.Guard;
using ProForma.Shared.Handlers;
using ProForma.Shared.Plugins;
using ProForma.Shared.Plugins.Extensions;

using Serilog;

using System.Drawing;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProForma;
//NOTE: To hide the console window, go to the project properties and change the Output Type to Windows Application.
// Or edit the .csproj file and change the <OutputType> tag from "WinExe" to "Exe".

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        Environment.SetEnvironmentVariable(
            "WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS",
            "--remote-debugging-port=9222"
        );
#endif
        var initConfiguration = ConfigurationFactory(args);
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(initConfiguration)
            .CreateLogger();

        var cts = new CancellationTokenSource();
        try
        {
            Log.Information("Starting ProForma");

            var services = new ServiceCollection();
            services.AddKeyedSingleton("plugin-settings-file", Path.Combine(AppContext.BaseDirectory, "settings", "plugins.config.json"));
            services.AddLogging(lb => lb.AddSerilog(Log.Logger));
            services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            services.AddSingleton<IFileServerBuilder, FileServerBuilder>();
            services.AddSingleton<IPluginConfigurationService, PluginConfigurationService>();
            services.AddSingleton<IPluginManagerService, PluginManagerService>();
            services.AddKeyedSingleton<IPluginInstanceFactory>("cs", (sp, _) =>
            {
                var factory = new PluginInstanceFactory();
                factory.SetPluginFactory((configItem, services) =>
                {
                    if (string.IsNullOrWhiteSpace(configItem.EntryFile) || !configItem.EntryFile.EndsWith(".dll"))
                        throw new DllNotFoundException($"No valid entry file specifies '{configItem.EntryFile}'.");

                    var assembly = Assembly.LoadFrom(Path.Combine(configItem.Path, configItem.EntryFile));
                    var pluginType = assembly.GetTypes().Where(t =>
                        !t.IsInterface
                        && t.GetInterface(nameof(IPlugin), true) != null).FirstOrDefault() 
                        ?? throw new TypeAccessException($"Plugin {configItem.Id}: Could not load a IPlugin implementation from entry file'{configItem.EntryFile}'.");

                    services.AddKeyedSingleton(typeof(IPlugin), configItem.Id, pluginType);
                    using var scope = services.BuildServiceProvider().CreateScope();
                    return scope.ServiceProvider.GetKeyedService<IPlugin>(configItem.Id) as IPlugin;
                });
                return factory;
            });
            services.AddKeyedSingleton<IPluginInstanceFactory>("js", (sp, _) =>
            {
                var factory = new PluginInstanceFactory();
                factory.SetPluginFactory((configItem, _) =>
                {
                    if (string.IsNullOrWhiteSpace(configItem.EntryFile) || !configItem.EntryFile.EndsWith(".js"))
                        throw new DllNotFoundException($"No valid entry file specifies '{configItem.EntryFile}'.");

                    var jsPlugin = new JsProviderPlugin(sp.GetService<Microsoft.Extensions.Logging.Logger<JsProviderPlugin>>()!);
                    jsPlugin.SetManifest(configItem.ToManifest());
                    jsPlugin.SetFile(Path.GetFullPath(Path.Combine(configItem.Path, configItem.EntryFile)));
                    return jsPlugin;
                });
                return factory;
            });           

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var pluginManagerService = scope.ServiceProvider.GetRequiredService<IPluginManagerService>();
                pluginManagerService.Scan(services);
                pluginManagerService.GetAllPlugins().ToList().ForEach(plugin => {
                    plugin.ConfigurePlugin(scope.ServiceProvider, ref services);                
                });
            }


            var appBaseDir = AppContext.BaseDirectory;

            using var serviceProvider = services.BuildServiceProvider();
                        
            var fileServerBuilder = serviceProvider.GetRequiredService<IFileServerBuilder>()
                .AddPort(8000, 100)
                .AddDirectory("plugins", Path.Join(appBaseDir, "Plugins"));
            var fileServer = fileServerBuilder.Build(args, out string baseUrl);
            fileServer.RunAsync().WaitAsync(cts.Token);

            // Window title declared here for visibility
            string windowTitle = $"ProForma";

            var window = new PhotinoWindow()
                .SetUseOsDefaultLocation(false)
                .SetUseOsDefaultSize(false)
                .Center()
                .SetSize(1024, 786)
                .SetTitle(windowTitle)
                .SetResizable(true)
                .SetChromeless(false);
            var customSchemeHandlers = serviceProvider.GetServices<IWindowCustomSchemeHandler>();
            foreach (var handler in customSchemeHandlers)
            {
                window.RegisterCustomSchemeHandler(handler.GetScheme(), (sender, scheme, url, out contentType) =>
                {
                    contentType = string.Empty;
                    if (handler.CanHandle(sender, scheme))
                    {
                        return handler.Handle(sender, scheme, url, out contentType);
                    }
                    return null;
                });
            }

            var webMessageReceivedHandlers = serviceProvider.GetServices<IWindowWebMessageReceivedHandler>();
            foreach(var handler in webMessageReceivedHandlers)
            {
                window.RegisterWebMessageReceivedHandler((sender, message) =>
                {
                    if (handler.CanHandle(sender, message))
                    {
                        handler.Handle(sender, message);
                    }
                });
            }
            window.Load(baseUrl);
            window.WaitForClose();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            cts.Cancel();
            Log.CloseAndFlush();
        }
    }

    private static IConfiguration ConfigurationFactory(string[] args)
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "settings", "app.config.json"), optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "settings", "plugins.config.json"), optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
    }
}
