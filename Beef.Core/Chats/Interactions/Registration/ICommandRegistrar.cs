using Discord;
using Discord.Interactions;
using Discord.Rest;

namespace Beef.Core.Chats.Interactions.Registration;

public interface ICommandRegistrar 
{
    Task<IReadOnlyCollection<RestGuildCommand>> RegisterCommandsToGuildAsync(ulong guildId, bool deleteMissing = true);
    Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandsGloballyAsync(bool deleteMissing = true);
    Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync(ModuleInfo module, IGuild guild, params ApplicationCommandPermission[] permissions);
    Task<GuildApplicationCommandPermission> ModifySlashCommandPermissionsAsync(SlashCommandInfo command, IGuild guild, params ApplicationCommandPermission[] permissions);
    IReadOnlyList<ModuleInfo> Modules { get; }
    IReadOnlyList<SlashCommandInfo> SlashCommands { get; }
}