using Beef.Core.Chats.Interactions.Registration;
using Beef.Core.Data;
using Beef.Core.Triggers;
using Discord;
using Discord.Interactions;
using RuntimeResult = Discord.Interactions.RuntimeResult;

namespace Beef.Core.Modules;

[Discord.Interactions.Group("test", "Test commands.")]
public class TestModule : InteractionModuleBase<IInteractionContext>
{
    private readonly ICommandRegistrar _commandRegistrar;
    private readonly ITriggerExecutor _triggerExecutor;

    public TestModule(ICommandRegistrar commandRegistrar, ITriggerExecutor triggerExecutor)
    {
        _commandRegistrar = commandRegistrar;
        _triggerExecutor = triggerExecutor;
    }

    [SlashCommand("ping", "Responds with pong.")]
    public async Task<RuntimeResult> Ping()
    {
        return CommandResult.Ok("Pong");
    }

    [SlashCommand("delay", "Responds after a delay.")]
    public async Task<RuntimeResult> Delay(TimeSpan delay)
    {
        await Task.Delay(delay);
        return CommandResult.Ok("Ok");
    }

    [SlashCommand("giveperm", "Gives permission.")]
    public async Task<RuntimeResult> GivePerms(IUser target, bool enable)
    {
        var command = _commandRegistrar.SlashCommands.FirstOrDefault(
            x => x.Module.Name == nameof(PermsModule) && x.Name == "perms"
        ) ?? throw new Exception("No command named perms was found.");
        await _commandRegistrar.ModifySlashCommandPermissionsAsync(
            command,
            Context.Guild,
            new ApplicationCommandPermission(target, enable)
        );
        return CommandResult.Ok();
    }

    [SlashCommand("exec", "Executes a command.")]
    public async Task<RuntimeResult> Execute(string command)
    {
        await _triggerExecutor.ExecuteAsync(
            new OneTimeTrigger(
                new TriggerContext(ChatType.Discord, Context.Guild.Id, Context.Channel.Id, Context.User.Id, command),
                DateTime.Now
            )
        );
        return CommandResult.Ok();
    }
}