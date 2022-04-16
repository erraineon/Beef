using System.Reflection;
using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public interface IInteractionService
{
    IReadOnlyList<SlashCommandInfo> SlashCommands { get; }
    IReadOnlyList<ModuleInfo> Modules { get; }
    Task<IResult> ExecuteCommandAsync(IInteractionContext context, IServiceProvider services);
    Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services);
}