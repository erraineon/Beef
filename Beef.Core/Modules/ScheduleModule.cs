using Beef.Core.Data;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Humanizer;

namespace Beef.Core.Modules;

public class ScheduleModule : InteractionModuleBase<IInteractionContext>
{
    [Group("remind", "Reminds users about stuff.")]
    public class RemindModule : InteractionModuleBase<IInteractionContext>
    {
        private readonly BeefDbContext _beefDbContext;

        public RemindModule(BeefDbContext beefDbContext)
        {
            _beefDbContext = beefDbContext;
        }

        [SlashCommand("in", "Posts a reminder at the given time.")]
        public async Task<RuntimeResult> Remind(TimeSpan timeSpan, string reminder)
        {
            var now = DateTime.UtcNow;
            var dateTime = now + timeSpan;
            var trigger = new OneTimeTrigger
            {
                ChannelId = Context.Channel.Id,
                ChatType = Context.Client is DiscordSocketClient ? ChatType.Discord : ChatType.Telegram,
                GuildId = Context.Guild.Id,
                UserId = Context.User.Id,
                GuildPermissionsRawValue = ((IGuildUser)Context.User).GuildPermissions.RawValue,
                TriggerAtUtc = dateTime,
                CommandToRun = $"echo {Context.User.Mention}: {reminder}"
            };
            await _beefDbContext.Triggers.AddAsync(trigger);
            await _beefDbContext.SaveChangesAsync();
            return CommandResult.Ok($"Will remind in {timeSpan.Humanize()} ({dateTime} UTC).");
        }
    }
}