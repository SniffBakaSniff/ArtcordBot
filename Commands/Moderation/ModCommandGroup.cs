using ArtcordBot.Services;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;

namespace ArtcordBot.Features.ModerationCommands
{
    [Command("mod")]
    [RequirePermissions(DiscordPermissions.Administrator)] // Placeholder till custom permissions handler is in place
    public partial class ModerationCommandGroup
    {

        private readonly IBanService _banService;
        private readonly IGuildSettingsService _guildSettingsService;
        private readonly IPaginationService _paginationService;
        private readonly IStringInterpolatorService _stringInterpolatorService;
        private readonly IGuildPresetService _guildPresetService;

        private readonly BotDbContext dbContext = new BotDbContext();

        public ModerationCommandGroup(
            IBanService banService,
            IGuildSettingsService guildSettingsService,
            IPaginationService paginationService,
            IStringInterpolatorService stringInterpolatorService,
            IGuildPresetService guildPresetService)
        {
            _banService = banService;
            _guildSettingsService = guildSettingsService;
            _httpClient = new HttpClient();
            _paginationService = paginationService;
            _stringInterpolatorService = stringInterpolatorService;
            _guildPresetService = guildPresetService;

        }
    }
}
