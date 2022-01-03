using Beef.Core.Data;

namespace Beef.Core.Chats;

public interface IChatService
{
    ChatType ChatType { get; }
}