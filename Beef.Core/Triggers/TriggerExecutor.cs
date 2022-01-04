using Beef.Core.Chats;
using Beef.Core.Chats.Interactions.Execution;
using Discord;

namespace Beef.Core.Triggers;

public class TriggerExecutor : ITriggerExecutor
{
    private readonly IChatClient _chatClient;
    private readonly IInteractionExecutor _interactionExecutor;
    private readonly IInteractionFactory _interactionFactory;

    public TriggerExecutor(
        IInteractionExecutor interactionExecutor,
        IChatClient chatClient,
        IInteractionFactory interactionFactory
    )
    {
        _interactionExecutor = interactionExecutor;
        _chatClient = chatClient;
        _interactionFactory = interactionFactory;
    }

    public async Task ExecuteAsync(Trigger trigger)
    {
        var triggerContext = trigger.Context;
        var textChannel = (IMessageChannel)await _chatClient.GetChannelAsync(triggerContext.ChannelId);
        var user = await _chatClient.GetUserAsync(triggerContext.UserId);
        var guild = await _chatClient.GetGuildAsync(triggerContext.GuildId);
        var triggerInteractionContext = new BotInteractionContext(
            _chatClient,
            guild,
            textChannel,
            user,
            _interactionFactory.CreateInteraction(user, textChannel, triggerContext.Command)
        );
        await _interactionExecutor.ExecuteInteractionAsync(triggerInteractionContext);
    }
}

public record BotInteractionContext(
    IDiscordClient Client,
    IGuild Guild,
    IMessageChannel Channel,
    IUser User,
    IDiscordInteraction Interaction
) : IInteractionContext;