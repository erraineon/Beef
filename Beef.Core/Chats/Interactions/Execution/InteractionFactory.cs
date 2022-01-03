using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public class InteractionFactory
{
    private readonly IInteractionService _interactionService;

    public InteractionFactory(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    public IDiscordInteraction? CreateInteraction(IUser user, ITextChannel channel, string text)
    {
        var targetCommand = FindTargetCommand(text);
    }

    private SlashCommandInfo? FindTargetCommand(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var tokens = new Queue<string>(text.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var candidates = new List<SlashCommandInfo>();

        do
        {
            // TODO: this should go through groups, then sub-groups, then commands.
            var currentToken = tokens.Dequeue();
            candidates.Clear();
            candidates.AddRange(
                _interactionService.SlashCommands
                    .Where(
                        x => string.Equals(
                            x.Name,
                            currentToken,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
            );
        } while (candidates.Count > 1 && tokens.Any());

        var target = candidates.FirstOrDefault();
        return target;
    }
}