using Discord;
using Discord.Interactions;

namespace Beef.Core.Interactions;

public class InteractionFactory(InteractionService interactionService) : IInteractionFactory
{
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
        var modulesToScan = interactionService.Modules;
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
            throw new Exception($"No command was found for: {text}");

        var parameterOptions = commandToRun.Parameters
            .Select(
                (parameter, i) =>
                {
                    if (!parameter.IsRequired && i >= tokensQueue.Count) return null;

                    var token = parameter.DiscordOptionType == ApplicationCommandOptionType.String &&
                        i == commandToRun.Parameters.Count - 1
                            ? string.Join(" ", tokensQueue.Skip(i))
                            : tokensQueue.ElementAt(i);

                    if (parameter.ParameterType.IsEnum)
                        token = Enum.TryParse(parameter.ParameterType, token, true, out var x) && x != null
                            ? x.ToString()
                            // TODO: convert to more specific type
                            : throw new Exception(
                                $"Failed to convert {token} to enum type {parameter.ParameterType.Name}."
                            );

                    return new BotInteractionDataOption(
                        parameter.Name,
                        token,
                        parameter.DiscordOptionType ?? ApplicationCommandOptionType.String,
                        null
                    );
                }
            )
            .Where(x => x != null)
            .Select(x => x!);

        currentOptionsList.AddRange(parameterOptions);
        return interactionData;
    }
}