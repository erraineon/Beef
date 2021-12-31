namespace Beef.Core.Chats.Discord;

public interface IChatClientLauncher
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}