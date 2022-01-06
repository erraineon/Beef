using Beef.Core.Chats.Interactions.Registration;
using Beef.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Beef.Core.Chats;

public class ChatServiceScopeFactory : IChatServiceScopeFactory
{
    private readonly IEnumerable<IChatClient> _chatClients;
    private readonly IEnumerable<ICommandRegistrar> _commandRegistrars;
    private readonly IServiceProvider _serviceProvider;

    public ChatServiceScopeFactory(
        IEnumerable<IChatClient> chatClients,
        IEnumerable<ICommandRegistrar> commandRegistrars,
        IServiceProvider serviceProvider
    )
    {
        _chatClients = chatClients;
        _commandRegistrars = commandRegistrars;
        _serviceProvider = serviceProvider;
    }

    public IServiceScope CreateScope(ChatType chatType)
    {
        var scope = _serviceProvider.CreateScope();
        var scopedContextHolder = scope.ServiceProvider.GetRequiredService<IChatContext>();
        scopedContextHolder.ChatClient = _chatClients.First(x => x.ChatType == chatType);
        scopedContextHolder.CommandRegistrar = _commandRegistrars.First(x => x.ChatType == chatType);
        return scope;
    }
}