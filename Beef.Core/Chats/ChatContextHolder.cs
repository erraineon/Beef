using Beef.Core.Chats.Interactions.Registration;
using Beef.Core.Data;

namespace Beef.Core.Chats;

public class ChatContextHolder : IChatContextHolder
{
    private readonly IEnumerable<IChatClient> _chatClients;
    private readonly IEnumerable<ICommandRegistrar> _commandRegistrars;
    private IChatContext? _context;

    public ChatContextHolder(
        IEnumerable<IChatClient> chatClients,
        IEnumerable<ICommandRegistrar> commandRegistrars
    )
    {
        _chatClients = chatClients;
        _commandRegistrars = commandRegistrars;
    }

    public IChatContext Context
    {
        get => _context ?? throw new InvalidOperationException("The scoped chat context has not been set.");
        private set
        {
            if (_context != null)
                throw new InvalidOperationException("The scoped chat context has already been set.");
            _context = value;
        }
    }

    public void SetCurrentContext(ChatType chatType)
    {
        Context = new ChatContext(
            _chatClients.First(x => x.ChatType == chatType),
            _commandRegistrars.First(x => x.ChatType == chatType)
        );
    }

    private record ChatContext(IChatClient ChatClient, ICommandRegistrar CommandRegistrar) : IChatContext;
}