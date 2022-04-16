using Beef.Core.Chats.Interactions.Registration;

namespace Beef.Core.Chats;

public interface IChatContext
{
    IChatClient ChatClient { get; }
    ICommandRegistrar CommandRegistrar { get; }
}