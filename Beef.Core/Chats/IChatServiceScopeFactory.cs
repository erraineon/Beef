using Beef.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Beef.Core.Chats;

public interface IChatServiceScopeFactory
{
    IServiceScope CreateScope(ChatType chatType);
}