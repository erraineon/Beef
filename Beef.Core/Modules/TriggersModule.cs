using System.Text;
using Beef.Core.Data;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Humanizer;

namespace Beef.Core.Modules;

public class TriggersModule : InteractionModuleBase<IInteractionContext>
{
    [Group("triggers", "Manages existing triggers.")]
    public class ManageTriggersModule(BeefDbContext beefDbContext) : InteractionModuleBase<IInteractionContext>
    {
        [SlashCommand("list", "Lists all registered triggers for this server.")]
        public async Task<RuntimeResult> ListTriggers()
        {
            var triggers = beefDbContext.Triggers.Where(x => x.GuildId == Context.Guild.Id).ToAsyncEnumerable();
            var sb = new StringBuilder();
            await foreach (var trigger in triggers)
            {
                var user = await Context.Client.GetUserAsync(trigger.UserId);
                var userName = user?.Username ?? "missing user";
                sb.Append($"{trigger.Id}: Run `{trigger.CommandToRun}` ");
                var stringToAppend = trigger switch
                {
                    OneTimeTrigger oneTime => $"at {oneTime.TriggerAtUtc} UTC",
                    _ => null
                };
                sb.Append(stringToAppend);
                sb.AppendLine($"For {userName}");
            }

            return new SuccessResult(sb.ToString());
        }

        [SlashCommand("remove", "Removes the trigger with the specified ID.")]
        public async Task<RuntimeResult> RemoveTrigger(int triggerId)
        {
            var trigger = await beefDbContext.Triggers.FindAsync(triggerId);
            if (trigger == null) return new SuccessResult($"No trigger with ID {triggerId} was found.");
            if (trigger.UserId != Context.User.Id) return new SuccessResult("You can only remove your own triggers.");
            beefDbContext.Triggers.Remove(trigger);
            await beefDbContext.SaveChangesAsync();
            return new SuccessResult();
        }
    }

    [Group("remind", "Reminds users about stuff.")]
    public class RemindModule(BeefDbContext beefDbContext) : InteractionModuleBase<IInteractionContext>
    {
        [SlashCommand("in", "Posts a reminder after the given time span is up.")]
        public async Task<RuntimeResult> Remind(TimeSpan timeSpan, string reminder)
        {
            var now = DateTime.UtcNow;
            var dateTime = now + timeSpan;
            await SaveReminderForTime(dateTime, reminder);
            return new SuccessResult($"Will remind in {timeSpan.Humanize()} ({dateTime} UTC).");
        }

        private async Task SaveReminderForTime(DateTime dateTime, string reminder)
        {
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
            await beefDbContext.Triggers.AddAsync(trigger);
            await beefDbContext.SaveChangesAsync();
        }

        [SlashCommand("on", "Posts a reminder at the given time.")]
        public async Task<RuntimeResult> Remind(DateTime dateTime, string reminder)
        {
            await SaveReminderForTime(dateTime, reminder);
            return new SuccessResult($"Will remind at {dateTime} UTC.");
        }
    }
}