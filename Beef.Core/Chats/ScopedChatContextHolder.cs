using Beef.Core.Chats.Interactions.Registration;

namespace Beef.Core.Chats;

class ScopedChatContextHolder : IScopedChatContextHolder
{
    private IChatClient? _chatClient;
    private ICommandRegistrar? _commandRegistrar;

    public IChatClient ChatClient
    {
        get => _chatClient ?? throw GetNotSetException();
        set => _chatClient = _chatClient == null ? value : throw GetAlreadySetException();
    }

    public ICommandRegistrar CommandRegistrar
    {
        get => _commandRegistrar ?? throw GetNotSetException();
        set => _commandRegistrar = _commandRegistrar == null ? value : throw GetAlreadySetException();
    }

    private static Exception GetNotSetException()
    {
        return new InvalidOperationException("The scoped chat context has not been set.");
    }

    private static Exception GetAlreadySetException()
    {
        return new InvalidOperationException("The scoped chat context has already been set.");
    }
}