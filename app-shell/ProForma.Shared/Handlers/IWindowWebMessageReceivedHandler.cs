namespace ProForma.Shared.Handlers;

public interface IWindowWebMessageReceivedHandler
{
    bool CanHandle(object? sender, string message);
    void Handle(object? sender, string message);
}
