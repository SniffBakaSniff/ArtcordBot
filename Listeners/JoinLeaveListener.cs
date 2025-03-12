using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using ArtcordBot.Services.Database;
using ArtcordBot.Services;

namespace ArtcordBot.Listeners
{
    public class JoinLeaveListener
    {
        private readonly IMessageSettingsService _ticketSettingsService;
        private readonly IGuildSettingsService _guildSettingsService;
        private readonly IStringInterpolatorService _stringInterpolatorService;

        public JoinLeaveListener(IMessageSettingsService messageSettingsService, IGuildSettingsService guildSettingsService, IStringInterpolatorService stringInterpolatorService)
        {
            _ticketSettingsService = messageSettingsService;
            _guildSettingsService = guildSettingsService;
            _stringInterpolatorService = stringInterpolatorService;
        }

        public async Task OnMemberJoined(DiscordClient client, GuildMemberAddedEventArgs e)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                string? welcomeMessage = await _ticketSettingsService.ManageMessageSettingAsync(e.Guild.Id, MessageType.Welcome);
                ulong? welcomeChannelId = await _guildSettingsService.GetWelcomeChannelAsync(e.Guild.Id);

                if (welcomeChannelId.HasValue && welcomeMessage is not null)
                {
                    var welcomeChannel = await e.Guild.GetChannelAsync(welcomeChannelId.Value);
                    
                    if (welcomeChannel is not null && welcomeChannel.Type == DiscordChannelType.Text)
                    {
                        var interpolatedMessage = _stringInterpolatorService.Interpolate(welcomeMessage, new EventContext(e));
                        await welcomeChannel.SendMessageAsync(interpolatedMessage);
                    }
                }
            });
        }

        public async Task OnMemberLeft(DiscordClient client, GuildMemberRemovedEventArgs e)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                string? farewellMessage = await _ticketSettingsService.ManageMessageSettingAsync(e.Guild.Id, MessageType.Farewell);
                ulong? farewellChannelId = await _guildSettingsService.GetFarewellChannelAsync(e.Guild.Id);
                ulong? welcomeChannelId = await _guildSettingsService.GetWelcomeChannelAsync(e.Guild.Id);
                
                if (farewellChannelId.HasValue && farewellMessage is not null)
                {
                    var farewellChannel = await e.Guild.GetChannelAsync(farewellChannelId.Value);

                    if (farewellChannel is not null && farewellChannel.Type == DiscordChannelType.Text)
                    {
                        var interpolatedMessage = _stringInterpolatorService.Interpolate(farewellMessage, new EventContext(e));
                        await farewellChannel.SendMessageAsync(interpolatedMessage);
                    }
                }
            });
        }
    }
}
