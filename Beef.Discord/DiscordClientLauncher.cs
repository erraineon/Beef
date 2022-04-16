using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Beef.Discord;

public class DiscordClientLauncher : IHostedService
{
    private readonly IOptions<DiscordOptions> _discordOptions;
    private DiscordSocketClient? _discordClient;

    public DiscordClientLauncher(IOptions<DiscordOptions> discordOptions)
    {
        _discordOptions = discordOptions;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _discordClient = new DiscordSocketClient(new DiscordSocketConfig { GatewayIntents = GatewayIntents.All });
        _discordClient.MessageReceived += OnMessageReceivedAsync;
        await _discordClient.LoginAsync(TokenType.Bot, _discordOptions.Value.Token);
        await _discordClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_discordClient != null) await _discordClient.StopAsync();
    }

    private async Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Content == "ping")
            await message.Channel.SendMessageAsync("pong", messageReference: message.Reference);
    }
}