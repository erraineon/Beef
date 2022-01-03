using Discord;

namespace Beef.Core.Chats.Interactions.Execution;

public class EmulatedInteractionData : IApplicationCommandInteractionData
{
    private EmulatedInteractionData(string name, IReadOnlyCollection<IApplicationCommandInteractionDataOption> options)
    {
        Name = name;
        Options = options;
    }

    public static EmulatedInteractionData FromCommand(string command)
    {
        return new EmulatedInteractionData(name, options);
    }

    public ulong Id => throw new NotImplementedException();
    public string Name { get; }
    public IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; }
}