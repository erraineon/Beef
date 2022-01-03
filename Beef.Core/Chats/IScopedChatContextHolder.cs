using Beef.Core.Chats.Interactions.Registration;

namespace Beef.Core.Chats;

public interface IScopedChatContextHolder
{
    IChatClient ChatClient { get; set; }
    ICommandRegistrar CommandRegistrar { get; set; }
}