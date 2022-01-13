using Beef.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Beef.Core.Chats;

public interface IChatScopeFactory
{
    IServiceScope CreateScope(ChatType chatType);
}