using Beef.Core.Modules;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Chats.Interactions.Execution;

public class InteractionHandler : IInteractionHandler
{
    private readonly IInteractionExecutor _interactionExecutor;
    private readonly ILogger<InteractionHandler> _logger;

    public InteractionHandler(IInteractionExecutor interactionExecutor, ILogger<InteractionHandler> logger)
    {
        _interactionExecutor = interactionExecutor;
        _logger = logger;
    }

    public async void HandleInteractionNoAwait(IInteractionContext context)
    {
        try
        {
            var interactionExecutionTask = _interactionExecutor.ExecuteInteractionAsync(context);
            var interactionResponseType = await DeferInteractionIfLateAsync(context, interactionExecutionTask);
            var result = await interactionExecutionTask;
            await HandleResultAsync(context, result, interactionResponseType);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while handling an interaction.");
        }
    }

    private static async Task<InteractionResponseType> DeferInteractionIfLateAsync(
        IInteractionContext context,
        Task<IResult> interactionExecutionTask
    )
    {
        // The actual timeout is 3 seconds, but shave a second off to account for network delays.
        var interactionTimeout = TimeSpan.FromSeconds(2);

        await Task.WhenAny(
            interactionExecutionTask,
            Task.Delay(interactionTimeout)
        );
        var interactionResponseType = interactionExecutionTask.IsCompleted
            ? InteractionResponseType.Reply
            : InteractionResponseType.FollowUp;

        if (interactionResponseType == InteractionResponseType.FollowUp)
            await context.Interaction.DeferAsync();
        return interactionResponseType;
    }

    private async Task HandleResultAsync(
        IInteractionContext context,
        IResult result,
        InteractionResponseType interactionResponseType
    )
    {
        try
        {
            if (result is ICommandResult commandResult)
            {
                var resultString = commandResult.Result.ToString();
                await (interactionResponseType switch
                {
                    InteractionResponseType.Reply => context.Interaction.RespondAsync(resultString),
                    InteractionResponseType.FollowUp => context.Interaction.FollowupAsync(resultString)
                });
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while handling an interaction's result.");
        }
    }

    private enum InteractionResponseType
    {
        Reply,
        FollowUp
    }
}