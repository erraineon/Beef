using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Beef.Core.Interactions;

public class InteractionHandler(
    InteractionService interactionService,
    IInteractionFactory interactionFactory,
    IServiceProvider serviceProvider,
    IEnumerable<IMessageContentPreprocessor> messageContentPreprocessors,
    ILogger<InteractionHandler> logger)
    : IInteractionHandler
{
    public void HandleMessage(IDiscordClient client, IUserMessage message)
    {
        try
        {
            const string commandPrefix = ".";
            var content = message.Content;
            if (content.Length > 1 && !message.Author.IsBot)
            {
                content = messageContentPreprocessors
                    .Select(x => x.GetProcessedInputOrNull(message))
                    .FirstOrDefault(x => x != default) ?? content;

                if (content.StartsWith(commandPrefix))
                {
                    var interaction = interactionFactory.CreateInteraction(
                        message.Author,
                        message.Channel,
                        content.Substring(commandPrefix.Length)
                    );
                    var interactionContext = new InteractionContext(client, interaction, message.Channel);
                    HandleInteractionContext(interactionContext);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while handling a message-based interaction.");
        }
    }

    public async void HandleInteractionContext(IInteractionContext context)
    {
        try
        {
            var interactionTask = ExecuteInteractionAsync(context);
            var isLate = await DeferInteractionIfLateAsync(context, interactionTask);
            var result = await interactionTask;

            if (result.Error is not (InteractionCommandError.UnknownCommand
                or InteractionCommandError.UnmetPrecondition))
                await HandleResultAsync(context, result, isLate);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while handling an interaction.");
        }
    }

    private async Task<IResult> ExecuteInteractionAsync(IInteractionContext context)
    {
        var scope = serviceProvider.CreateScope();
        var result = await interactionService.ExecuteCommandAsync(context, scope.ServiceProvider);
        if (!result.IsSuccess)
        {
            var exception = (result is ExecuteResult executeResult)
                ? executeResult.Exception
                : new Exception($"{result.Error}: {result.ErrorReason}");
            logger.LogError(exception, "Error while executing an interaction.");
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
        const string errorMessage = "👎";
        try
        {
            var contentToReplyWith = result switch
            {
                ExecuteResult { Exception: ModuleException e } => e.Message,
                SuccessResult { Result: IEnumerable<object> r } => string.Join('\n', r),
                SuccessResult { Result: var r and not (null or "") } => r.ToString(),
                ExecuteResult { IsSuccess: true } or SuccessResult => "👌",
                _ => errorMessage
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
            logger.LogError(e, "Error while handling an interaction's result.");
            try
            {
                // Handle cases like an interaction response being too long by at least giving some kind of response.
                await (isLate
                    ? context.Interaction.FollowupAsync(errorMessage)
                    : context.Interaction.RespondAsync(errorMessage));
            }
            catch
            {
            }
        }
    }
}