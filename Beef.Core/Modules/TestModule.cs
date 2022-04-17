using Discord;
using Discord.Interactions;

namespace Beef.Core.Modules;

[Group("test", "Test commands.")]
public class TestModule : InteractionModuleBase<IInteractionContext>
{
    private readonly IInteractionHandler _interactionHandler;
    private readonly IInteractionFactory _interactionFactory;

    public TestModule(IInteractionHandler interactionHandler, IInteractionFactory interactionFactory)
    {
        _interactionHandler = interactionHandler;
        _interactionFactory = interactionFactory;
    }
    [SlashCommand("ping", "Responds with pong.")]
    public async Task<RuntimeResult> Ping()
    {
        return CommandResult.Ok("Pong");
    }

    [SlashCommand("add", "Adds two numbers.")]
    public async Task<RuntimeResult> Ping(int a, string b)
    {
        return CommandResult.Ok(a + int.Parse(b));
    }

    [SlashCommand("delay", "Responds after a delay.")]
    public async Task<RuntimeResult> Delay(TimeSpan delay)
    {
        await Task.Delay(delay);
        return CommandResult.Ok("Ok");
    }

    [SlashCommand("exec", "Executes a command.")]
    public async Task<RuntimeResult> Execute(string command)
    {
        var interaction = _interactionFactory.CreateInteraction(Context.User, Context.Channel, command);
        var interactionContext = new InteractionContext(Context.Client, interaction, Context.Channel);
        _interactionHandler.HandleInteractionContext(interactionContext);
        return CommandResult.Ok();
    }
}