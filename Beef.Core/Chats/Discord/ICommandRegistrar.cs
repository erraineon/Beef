namespace Beef.Core.Chats.Discord;

public interface ICommandRegistrar
{
    Task RegisterCommandsAsync(CancellationToken cancellationToken);
}