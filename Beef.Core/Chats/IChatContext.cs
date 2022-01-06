using Beef.Core.Chats.Interactions.Registration;

namespace Beef.Core.Chats;

public interface IChatContext
{
    IChatClient ChatClient { get; set; }
    ICommandRegistrar CommandRegistrar { get; set; }
}