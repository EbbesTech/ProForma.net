using System.Dynamic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Builder;

namespace ProForma.Shared.FileServer;

/// <summary>
/// Interface for File Server Builder.
/// </summary>
public interface IFileServerBuilder
{
    /// <summary>
    /// Add a portgrange.
    /// </summary>
    /// <param name="startPort">Port for starting the search.</param>
    /// <param name="portRange">Number of ports to incremental test.</param>
    /// <returns>Instance of self.</returns>
    public IFileServerBuilder AddPort(int startPort, int portRange);

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
    /// <param name="urlPrefix"></param>
    /// <param name="physicalPath"></param>
    /// <returns>Instance of self.</returns>
    public IFileServerBuilder AddDirectory(string urlPrefix, string physicalPath);

    /// <summary>
    /// Build the WebApplication with the given configuration.
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <returns>WebApplication</returns>
    public WebApplication Build(string[] args, out string baseUrl);
}
