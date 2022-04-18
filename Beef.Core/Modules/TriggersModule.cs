﻿using System.Text;
using Beef.Core.Data;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Humanizer;

namespace Beef.Core.Modules;

public class TriggersModule : InteractionModuleBase<IInteractionContext>
{
    private readonly BeefDbContext _beefDbContext;

    public TriggersModule(BeefDbContext beefDbContext)
    {
        _beefDbContext = beefDbContext;
    }

    [Group("triggers", "Manages existing triggers.")]
    public class ManageTriggersModule : InteractionModuleBase<IInteractionContext>
    {
        private readonly BeefDbContext _beefDbContext;

        public ManageTriggersModule(BeefDbContext beefDbContext)
        {
            _beefDbContext = beefDbContext;
        }

        [SlashCommand("list", "Lists all registered triggers for this server.")]
        public async Task<RuntimeResult> ListTriggers()
        {
            var triggers = _beefDbContext.Triggers.Where(x => x.GuildId == Context.Guild.Id).ToAsyncEnumerable();
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

            return CommandResult.Ok(sb.ToString());
        }

        [SlashCommand("remove", "Removes the trigger with the specified ID.")]
        public async Task<RuntimeResult> RemoveTrigger(int triggerId)
        {
            var trigger = await _beefDbContext.Triggers.FindAsync(triggerId);
            if (trigger == null) return CommandResult.Ok($"No trigger with ID {triggerId} was found.");
            if (trigger.UserId != Context.User.Id) return CommandResult.Ok("You can only remove your own triggers.");
            _beefDbContext.Triggers.Remove(trigger);
            await _beefDbContext.SaveChangesAsync();
            return CommandResult.Ok();
        }
    }

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