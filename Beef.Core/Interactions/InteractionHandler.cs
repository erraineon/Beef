using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Interactions;

public class InteractionHandler : IInteractionHandler
{
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InteractionHandler> _logger;

    public InteractionHandler(
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        ILogger<InteractionHandler> logger
    )
    {
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async void HandleInteractionContext(IInteractionContext context)
    {
        try
        {
            var interactionTask = ExecuteInteractionAsync(context);
            var isLate = await DeferInteractionIfLateAsync(context, interactionTask);
            var result = await interactionTask;
            await HandleResultAsync(context, result, isLate);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while handling an interaction.");
        }
    }

    private async Task<IResult> ExecuteInteractionAsync(IInteractionContext context)
    {
        var scope = _serviceProvider.CreateScope();
        var result = await _interactionService.ExecuteCommandAsync(context, scope.ServiceProvider);
        return result;
    }

    private static async Task<bool> DeferInteractionIfLateAsync(
        IInteractionContext context,
        Task interactionTask
    )
    {
        // The actual timeout is 3 seconds, but shave a second off to account for network delays.
        var interactionTimeout = TimeSpan.FromSeconds(2);

        await Task.WhenAny(
            interactionTask,
            Task.Delay(interactionTimeout)
        );

        var isLate = !interactionTask.IsCompleted;
        if (isLate) await context.Interaction.DeferAsync();
        return isLate;
    }

    private async Task HandleResultAsync(
        IInteractionContext context,
        IResult result,
        bool isLate
    )
    {
        try
        {
            var contentToReplyWith = result is CommandResult commandResult ? commandResult.Result.ToString() :
                !result.IsSuccess ? result.ErrorReason :
                throw new Exception($"Unknown interaction result type: {result}");
            await (isLate
                ? context.Interaction.FollowupAsync(contentToReplyWith)
                : context.Interaction.RespondAsync(contentToReplyWith));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while handling an interaction's result.");
        }
    }
}