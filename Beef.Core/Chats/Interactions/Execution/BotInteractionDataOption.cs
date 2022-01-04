using Discord;

namespace Beef.Core.Chats.Interactions.Execution;

public class BotInteractionDataOption : IApplicationCommandInteractionDataOption
{
    public BotInteractionDataOption(
        string value,
        ApplicationCommandOptionType type,
        IReadOnlyCollection<IApplicationCommandInteractionDataOption> options
    )
    {
        Name = value;
        Value = value;
        Type = type;
        Options = options;
    }

    public string Name { get; }
    public object Value { get; }
    public ApplicationCommandOptionType Type { get; }
    public IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
}