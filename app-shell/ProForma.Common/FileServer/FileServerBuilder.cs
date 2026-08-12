using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using ProForma.Common.Extensions;
using ProForma.Shared.FileServer;

using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Serilog;
using System.ComponentModel;
using ProForma.Common.Guard;
using System.Data;

namespace ProForma.Common.FileServer;

/// <summary>
/// Builder for a small file server.
/// You can add directories, from where the static files are served. 
/// wwwroot is added by default.  
/// </summary>
public class FileServerBuilder(ILogger<FileServerBuilder> _logger) : IFileServerBuilder
{    
    private readonly Dictionary<string, string> _directories = [];
    private int _startPort = 8000;
    private int _portRage = 100;

    /// <summary>
    /// Adds a directory for serving the static files in it.
    /// Example:
    /// You have a folder assets/ with a image.png in it. 
    /// You register like: 
    /// <code>
    /// fileServerBuilder.AddDirectory("assets", $"{applicationBaseDir}/assets/")
    /// </code>
    /// Later you can load image.png with a url like http://localhost:8000/assets/image.png
    /// </summary>
    /// <param name="startPort"></param>
    /// <param name="portRange"></param>
    /// <returns>Instance of self.</returns>
    /// <exception cref="Exception">When key or value already exists.</exception>
    public IFileServerBuilder AddDirectory(string urlPrefix, string physicalPath)
    {
        _logger.LogDebug($"Call {nameof(FileServerBuilder)}.{nameof(AddDirectory)}.");
        Guard<DirectoryNotFoundException>.IsFalse(Directory.Exists(physicalPath)).Throw($"Could not find the given path '{physicalPath}'.");
        Guard<DuplicateNameException>.IsTrue(_directories.ContainsKey(urlPrefix)).Throw($"Key '{urlPrefix}' already exists.");
        Guard<Exception>.IsTrue(_directories.ContainsValue(physicalPath)).Throw($"Physical Path '{physicalPath}' already exists.");

        _directories.Add(urlPrefix, physicalPath);
        return this;
    }

    /// <summary>
    /// Add a portgrange.
    /// </summary>
    /// <param name="startPort">Port for starting the search.</param>
    /// <param name="portRange">Number of ports to incremental test.</param>
    /// <returns>Instance of self.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When startPort+portRage exceed ushort.MaxValue.</exception>
    public IFileServerBuilder AddPort(int startPort, int portRange)
    {
        _logger.LogDebug($"Call {nameof(FileServerBuilder)}.{nameof(AddPort)}.");
        Guard<ArgumentOutOfRangeException>.IsLessThanOrEqualTo(startPort + portRange, ushort.MaxValue)
            .Throw($"The given range of {portRange} plus the start port of {startPort} exceeds the maximum range of {ushort.MaxValue}");

        _startPort = startPort;
        _portRage = portRange;
        return this;
    }

    /// <summary>
    /// Build the WebApplication with the given configuration.
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <returns>WebApplication</returns>
    public WebApplication Build(string[] args, out string baseUrl)
    {
        _logger.LogDebug($"Call {nameof(FileServerBuilder)}.{nameof(Build)}.");
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args,
            
        });
        baseUrl = $"http://localhost:{FindFreePort()}";
        builder.WebHost.UseUrls(baseUrl);
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);

        var app = builder.Build();
        app.UseFileServer();
        _directories.ForEach((key, value) =>
        {
            var options = new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(value),
                RequestPath = $"/{key}"
            };
            options.DefaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseFileServer(options);
        });
        return app;
    }

    /// <summary>
    /// Find a free port in the given range.
    /// </summary>
    /// <returns>First free found port</returns>
    /// <exception cref="ArgumentOutOfRangeException">If none free port was found in the given range.</exception>
    private int FindFreePort()
    {
        _logger.LogDebug($"Call {nameof(FileServerBuilder)}.{nameof(FindFreePort)}.");
        int currentPort = _startPort;
        while(IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()
            .Any(listener => listener.Port == currentPort))
        {
            if (currentPort > _startPort + _portRage) throw new ArgumentOutOfRangeException($"No free port was found in the given range from {_startPort} to {_startPort + _portRage}.");
            ++currentPort;
        }
        return currentPort;
    }
}