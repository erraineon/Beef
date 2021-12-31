using Discord;
using Discord.Interactions;

namespace Beef.Core.Chats.Discord;

public class DiscordLauncher : IDiscordLauncher
{
    private readonly IDiscordOptions _discordOptions;
    private readonly IDiscordChatClient _discordChatClient;

    public DiscordLauncher(IDiscordOptions discordOptions, IDiscordChatClient discordChatClient)
    {
        _discordOptions = discordOptions;
        _discordChatClient = discordChatClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var discordReady = new TaskCompletionSource();

        Task OnReady()
        {
            _discordChatClient.Ready -= OnReady;
            discordReady.SetResult();
            return Task.CompletedTask;
        }

        _discordChatClient.Ready += OnReady;
        await _discordChatClient.LoginAsync(TokenType.Bot, _discordOptions.Token);
        await _discordChatClient.StartAsync();
        await discordReady.Task;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordChatClient.StopAsync();
    }
}