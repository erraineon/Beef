using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public class InteractionExecutor : IInteractionExecutor
{
    private readonly IChatScopeFactory _chatScopeFactory;
    private readonly IInteractionService _interactionService;

    public InteractionExecutor(IInteractionService interactionService, IChatScopeFactory chatScopeFactory)
    {
        _interactionService = interactionService;
        _chatScopeFactory = chatScopeFactory;
    }

    public async Task<IResult> ExecuteInteractionAsync(IInteractionContext interactionContext)
    {
        var scope = _chatScopeFactory.CreateScope(((IChatClient) interactionContext.Client).ChatType);
        var result = await _interactionService.ExecuteCommandAsync(interactionContext, scope.ServiceProvider);
        return result;
    }
}