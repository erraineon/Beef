using Beef.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Beef.Core.Chats;

public class ChatScopeFactory : IChatScopeFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ChatScopeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IServiceScope CreateScope(ChatType chatType)
    {
        var scope = _serviceProvider.CreateScope();
        var chatContextHolder = scope.ServiceProvider.GetRequiredService<IChatContextHolder>();
        chatContextHolder.SetCurrentContext(chatType);
        return scope;
    }
}