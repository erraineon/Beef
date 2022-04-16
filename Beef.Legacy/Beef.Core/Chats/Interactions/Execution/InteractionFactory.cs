using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public class InteractionFactory : IInteractionFactory
{
    private readonly IInteractionService _interactionService;

    public InteractionFactory(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    public IDiscordInteraction CreateInteraction(IUser user, IMessageChannel channel, string text)
    {
        var interactionData = CreateInteractionData(text);
        return new BotInteraction(user, channel, interactionData);
    }

    private BotInteractionData CreateInteractionData(string text)
    {
        var tokens = new Queue<string>(text.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var firstToken = tokens.TryDequeue(out var x) ? x : string.Empty;
        var subModules = FilterSubModules(_interactionService.Modules, firstToken);
        var options = CreateInteractionDataOptions(subModules, tokens).ToList();
        var interactionData = new BotInteractionData(firstToken, options);
        return interactionData;
    }

    private List<ModuleInfo> FilterSubModules(
        IReadOnlyCollection<ModuleInfo> modules,
        string token
    )
    {
        var candidateCommand = modules
            .SelectMany(x => x.SlashCommands)
            .FirstOrDefault(
                x => string.Equals(
                    x.Name,
                    token,
                    StringComparison.OrdinalIgnoreCase
                )
            );
        var filteredModules = modules
            .SelectMany(x => x.SubModules)
            .Where(
                x => string.Equals(
                    x.SlashGroupName,
                    token,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            .ToList();
        return filteredModules;
    }

    private IEnumerable<IApplicationCommandInteractionDataOption> CreateInteractionDataOptions(
        IReadOnlyCollection<ModuleInfo> modules,
        Queue<string> tokens
    )
    {
        if (!modules.Any())
            // We found a command, so we just need to list out the options.
            foreach (var valueToken in tokens)
                yield return new BotInteractionDataOption(
                    valueToken,
                    ApplicationCommandOptionType.String,
                    Array.Empty<IApplicationCommandInteractionDataOption>()
                );
        if (tokens.TryDequeue(out var subToken))
        {
            var subModules = FilterSubModules(modules, subToken);
            yield return new BotInteractionDataOption(
                subToken,
                subModules.Any()
                    ? ApplicationCommandOptionType.SubCommandGroup
                    : ApplicationCommandOptionType.SubCommand,
                CreateInteractionDataOptions(subModules, tokens).ToList()
            );
        }
    }
}