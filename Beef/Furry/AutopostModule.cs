using Beef.Core;
using Beef.Core.Data;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Beef.Furry;

[RequireNsfw]
public class AutopostModule(IE621SearchEngine e621SearchEngine, BeefDbContext dbContext)
    : InteractionModuleBase<IInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [SlashCommand("autopost", "Creates an E621 autopost schedule.")]
    public async Task SaveReminderForTime(TimeSpan interval, string tags)
    {
        var trigger = new RecurringTrigger
        {
            ChannelId = Context.Channel.Id,
            ChatType = Context.Client is DiscordSocketClient ? ChatType.Discord : ChatType.Telegram,
            GuildId = Context.Guild.Id,
            UserId = Context.User.Id,
            GuildPermissionsRawValue = ((IGuildUser)Context.User).GuildPermissions.RawValue,
            Interval = interval,
            CommandToRun = $"latest {tags}"
        };
        trigger.Advance(DateTime.UtcNow);
        await dbContext.Triggers.AddAsync(trigger);
        await dbContext.SaveChangesAsync();
    }

    [SlashCommand("latest", "Posts the latest new results for the specified tags from E621.")]
    public async Task<RuntimeResult> SearchLatestAsync(string tags)
    {
        var posts = await e621SearchEngine.SearchLatestAsync(Context.Channel.Id.ToString(), tags);
        return new SuccessResult(posts.Select(x => $"https://e621.net/posts/{x.Id}"));
    }
}