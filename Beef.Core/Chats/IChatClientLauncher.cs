namespace Beef.Core.Chats;

public interface IChatClientLauncher
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}