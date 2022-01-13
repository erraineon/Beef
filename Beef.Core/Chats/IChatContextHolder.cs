using Beef.Core.Data;

namespace Beef.Core.Chats;

public interface IChatContextHolder
{
    IChatContext Context { get; }
    void SetCurrentContext(ChatType chatType);
}