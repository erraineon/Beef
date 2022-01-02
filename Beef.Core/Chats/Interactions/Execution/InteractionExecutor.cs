using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public class InteractionExecutor : IInteractionExecutor
{
    private readonly IInteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;

    public InteractionExecutor(IInteractionService interactionService, IServiceProvider serviceProvider)
    {
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
    }

    public async Task<IResult> ExecuteInteractionAsync(IInteractionContext interactionContext)
    {
        var result = await _interactionService.ExecuteCommandAsync(interactionContext, _serviceProvider);
        return result;
    }
}