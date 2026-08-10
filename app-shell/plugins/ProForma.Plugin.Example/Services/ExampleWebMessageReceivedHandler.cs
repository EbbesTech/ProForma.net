using ProForma.Shared.Handlers;
using Photino.NET;
using Microsoft.Extensions.Logging;

namespace ProForma.Plugin.Example.Services;

public class ExampleWebMessageReceivedHandler(ILogger<ExampleSchemeHandler> _logger) : IWindowWebMessageReceivedHandler
{
    public bool CanHandle(object? sender, string message)
    {
        _logger.LogDebug($"Call {nameof(ExampleWebMessageReceivedHandler)}.{CanHandle}");
        return true;
    }

    public void Handle(object? sender, string message)
    {
        _logger.LogDebug($"Call {nameof(ExampleWebMessageReceivedHandler)}.{Handle}");
        if (sender is not PhotinoWindow)
        {
            throw new InvalidOperationException("Sender must be a Proforma Window.");
        }
        var window = (PhotinoWindow)sender;

        // The message argument is coming in from sendMessage.
        // "window.external.sendMessage(message: string)"
        string response = $"Received message: \"{message}\"";

        // Send a message back the to JavaScript event handler.
        // "window.external.receiveMessage(callback: Function)"
        window.SendWebMessage(response);
    }
}