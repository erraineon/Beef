using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public class InteractionExecutor : IInteractionExecutor
{
    private readonly IChatServiceScopeFactory _chatServiceScopeFactory;
    private readonly IInteractionService _interactionService;

    public InteractionExecutor(IInteractionService interactionService, IChatServiceScopeFactory chatServiceScopeFactory)
    {
        _interactionService = interactionService;
        _chatServiceScopeFactory = chatServiceScopeFactory;
    }

    public async Task<IResult> ExecuteInteractionAsync(IInteractionContext interactionContext)
    {
        var scope = _chatServiceScopeFactory.CreateScope(((IChatClient)interactionContext.Client).ChatType);
        var result = await _interactionService.ExecuteCommandAsync(interactionContext, scope.ServiceProvider);
        return result;
    }
}