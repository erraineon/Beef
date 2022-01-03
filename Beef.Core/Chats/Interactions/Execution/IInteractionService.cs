using System.Reflection;
using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Interactions.Execution;

public interface IInteractionService 
{
    Task<IResult> ExecuteCommandAsync(IInteractionContext context, IServiceProvider services);
    Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services);
    IReadOnlyList<SlashCommandInfo> SlashCommands { get; }
}