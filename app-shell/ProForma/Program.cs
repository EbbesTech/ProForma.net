using Photino.NET;
using Photino.NET.Server;

using System.Drawing;
using System.Text;

namespace Photino.HelloPhotino.StaticFileServer;
//NOTE: To hide the console window, go to the project properties and change the Output Type to Windows Application.
// Or edit the .csproj file and change the <OutputType> tag from "WinExe" to "Exe".

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        PhotinoServer
            .CreateStaticFileServer(args, out string baseUrl)
            .RunAsync();

        // Window title declared here for visibility
        string windowTitle = $"ProForma: {baseUrl} {args}";

        // Creating a new PhotinoWindow instance with the fluent API

        var window = new PhotinoWindow()
            .SetLocation(new Point(100, 100))
            .SetSize(1024, 786)
            .SetTitle(windowTitle)
            .SetResizable(true)
            .SetChromeless(false)
            .RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
            {
                contentType = "text/javascript";
                return new MemoryStream(Encoding.UTF8.GetBytes(@"
                        (() =>{
                            window.setTimeout(() => {
                                alert(`🎉 Dynamically inserted JavaScript.`);
                            }, 1000);
                        })();
                    "));
            })
            // Most event handlers can be registered after the
            // PhotinoWindow was instantiated by calling a registration 
            // method like the following RegisterWebMessageReceivedHandler.
            // This could be added in the PhotinoWindowOptions if preferred.
            .RegisterWebMessageReceivedHandler((object sender, string message) =>
            {
                var window = (PhotinoWindow)sender;

                // The message argument is coming in from sendMessage.
                // "window.external.sendMessage(message: string)"
                string response = $"Received message: \"{message}\"";

                // Send a message back the to JavaScript event handler.
                // "window.external.receiveMessage(callback: Function)"
                window.SendWebMessage(response);
            })
            .Load($"{baseUrl}/index.html");
        window.WaitForClose();
    }
}
