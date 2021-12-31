using System.Reflection;
using Discord.Interactions;
using Discord.Rest;

namespace Beef.Core.Chats.Discord;

public interface IInteractionService 
{
    Task<IEnumerable<ModuleInfo>> AddModulesAsync(Assembly assembly, IServiceProvider services);
    Task<IReadOnlyCollection<RestGuildCommand>> RegisterCommandsToGuildAsync(ulong guildId, bool deleteMissing = true);
    Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandsGloballyAsync(bool deleteMissing = true);
}