using Beef.Core.Chats.Interactions.Registration;
using Discord;
using Discord.Interactions;

namespace Beef.Core.Modules;

[Group("test", "Test commands.")]
public class TestModule : InteractionModuleBase<IInteractionContext>
{
    private readonly ICommandRegistrar _commandRegistrar;

    public TestModule(ICommandRegistrar commandRegistrar)
    {
        _commandRegistrar = commandRegistrar;
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
        return CommandResult.Ok("Ok");
    }
}