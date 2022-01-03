using Beef.Core.Chats;
using Beef.Core.Chats.Interactions.Execution;
using Discord;
using Discord.Interactions;

namespace Beef.Core.Triggers;

public class TriggerExecutor : ITriggerExecutor
{
    private readonly IInteractionExecutor _interactionExecutor;
    private readonly IChatClient _chatClient;

    public TriggerExecutor(IInteractionExecutor interactionExecutor,
        IChatClient chatClient)
    {
        _interactionExecutor = interactionExecutor;
        _chatClient = chatClient;
    }
    public async Task ExecuteAsync(Trigger trigger)
    {
        var triggerContext = trigger.Context;
        var textChannel = (IMessageChannel)await _chatClient.GetChannelAsync(triggerContext.ChannelId);
        var user = await _chatClient.GetUserAsync(triggerContext.UserId);
        var triggerInteractionContext = new InteractionContext(
            _chatClient,
            new EmulatedInteraction(
                textChannel,
                user,
                EmulatedInteractionData.FromCommand(triggerContext.Command)
            ),
            user,
            textChannel
        );
        await _interactionExecutor.ExecuteInteractionAsync(triggerInteractionContext);
    }

    
}