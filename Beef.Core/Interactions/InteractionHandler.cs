using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Interactions;

public class InteractionHandler : IInteractionHandler
{
    private readonly InteractionService _interactionService;
    private readonly ILogger<InteractionHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

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

            if (result.Error != InteractionCommandError.UnknownCommand)
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
        if (!result.IsSuccess)
        {
            var exception = (result is ExecuteResult executeResult)
                ? executeResult.Exception
                : new Exception($"{result.Error}: {result.ErrorReason}");
            _logger.LogError(exception, "Error while executing an interaction.");
        }
        return result;
    }

    private static async Task<bool> DeferInteractionIfLateAsync(
        IInteractionContext context,
        Task interactionTask
    )
    {
        // The actual timeout is 3 seconds.
        var interactionTimeout = TimeSpan.FromSeconds(1);

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
            var contentToReplyWith = result switch
            {
                ExecuteResult { Exception: ModuleException e } => e.Message,
                SuccessResult { Result: IEnumerable<object> r } => string.Join('\n', r),
                SuccessResult { Result: var r and not (null or "") } => r.ToString(),
                ExecuteResult { IsSuccess: true } or SuccessResult => "👌",
                _ => "👎"
            };

            var suppressResponse = context.Interaction is BotInteraction && string.IsNullOrWhiteSpace(contentToReplyWith);

            if (!suppressResponse)
            {
                if (string.IsNullOrWhiteSpace(contentToReplyWith))
                    contentToReplyWith = "👌";

                await (isLate
                    ? context.Interaction.FollowupAsync(contentToReplyWith)
                    : context.Interaction.RespondAsync(contentToReplyWith));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while handling an interaction's result.");
        }
    }
}