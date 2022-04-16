using Beef.Core.Data;

namespace Beef.Core.Chats;

public interface IChatComponent
{
    ChatType ChatType { get; }
}