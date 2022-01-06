using Beef.Core.Chats;
using Beef.Core.Chats.Interactions.Execution;
using Discord;

namespace Beef.Core.Triggers;

public class TriggerExecutor : ITriggerExecutor
{
    private readonly IInteractionExecutor _interactionExecutor;
    private readonly IChatContext _chatContext;
    private readonly IInteractionFactory _interactionFactory;

    public TriggerExecutor(
        IInteractionExecutor interactionExecutor,
        IChatContext chatContext,
        IInteractionFactory interactionFactory
    )
    {
        _interactionExecutor = interactionExecutor;
        _chatContext = chatContext;
        _interactionFactory = interactionFactory;
    }

    public async Task ExecuteAsync(Trigger trigger)
    {
        var triggerContext = trigger.Context;
        var chatClient = _chatContext.ChatClient;
        var textChannel = (IMessageChannel)await chatClient.GetChannelAsync(triggerContext.ChannelId);
        var user = await chatClient.GetUserAsync(triggerContext.UserId);
        var guild = await chatClient.GetGuildAsync(triggerContext.GuildId);
        var triggerInteractionContext = new BotInteractionContext(
            chatClient,
            guild,
            textChannel,
            user,
            _interactionFactory.CreateInteraction(user, textChannel, triggerContext.Command)
        );
        await _interactionExecutor.ExecuteInteractionAsync(triggerInteractionContext);
    }
}