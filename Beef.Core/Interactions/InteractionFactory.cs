using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;

namespace Beef.Core.Interactions;

public class InteractionFactory : IInteractionFactory
{
    private readonly InteractionService _interactionService;

    public InteractionFactory(InteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    public IDiscordInteraction CreateInteraction(IUser user, IMessageChannel channel, string command)
    {
        var interactionData = CreateInteractionData(command);
        return new BotInteraction(user, channel, interactionData);
    }

    private IApplicationCommandInteractionData CreateInteractionData(string text)
    {
        var allTokens = text.SplitBySpaceAndQuotes();
        var tokensQueue = new Queue<string>(allTokens);
        SlashCommandInfo? commandToRun = default;
        BotInteractionData? interactionData = default;
        var currentOptionsList = new List<IApplicationCommandInteractionDataOption>();
        var modulesToScan = _interactionService.Modules;
        while (tokensQueue.Any() && commandToRun == default && modulesToScan.Any())
        {
            var token = tokensQueue.Dequeue();

            commandToRun = modulesToScan
                .SelectMany(x => x.SlashCommands)
                .FirstOrDefault(
                    x => string.Equals(
                        x.Name,
                        token,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            if (interactionData == null)
                interactionData = new BotInteractionData(token, currentOptionsList);
            else
                currentOptionsList.Add(
                    new BotInteractionDataOption(
                        token,
                        null,
                        commandToRun != null
                            ? ApplicationCommandOptionType.SubCommand
                            : ApplicationCommandOptionType.SubCommandGroup,
                        currentOptionsList = new List<IApplicationCommandInteractionDataOption>()
                    )
                );

            if (commandToRun == null)
            {
                var filteredModules = modulesToScan
                    .Where(
                        x => x.IsSlashGroup && string.Equals(
                            x.SlashGroupName,
                            token,
                            StringComparison.OrdinalIgnoreCase
                        )
                    ).ToList();

                modulesToScan = filteredModules;
            }
        }

        if (commandToRun == null || commandToRun.Parameters.Count(x => x.IsRequired) > tokensQueue.Count ||
            interactionData == null)
            throw new Exception($"No command was found for {text}");

        currentOptionsList.AddRange(
            commandToRun.Parameters
                .Select(
                    (parameter, i) =>
                    {
                        if (!parameter.IsRequired && i >= tokensQueue.Count) return null;

                        var token = parameter.DiscordOptionType == ApplicationCommandOptionType.String &&
                            i == commandToRun.Parameters.Count - 1
                                ? string.Join(" ", tokensQueue.Skip(i))
                                : tokensQueue.ElementAt(i);

                        return new BotInteractionDataOption(
                            parameter.Name,
                            token,
                            parameter.DiscordOptionType ?? ApplicationCommandOptionType.String,
                            null
                        );
                    }
                )
                .Where(x => x != null)
                .Select(x => x!)
        );

        return interactionData;
    }
}