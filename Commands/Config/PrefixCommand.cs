using DSharpPlus.Commands;
using ArtcordBot.Helpers;


namespace ArtcordBot.Features.ConfigCommands
{
    public partial class ConfigCommandsGroup
    {

        [Command("setprefix")]
        [System.ComponentModel.Description("Set the bot's prefix")]
        public async Task SetPrefixAsync(CommandContext ctx, string newPrefix)
        {
            if (string.IsNullOrWhiteSpace(newPrefix))
            {
                await ctx.RespondAsync("Prefix cannot be empty.");
                return;
            }

            if (newPrefix.Length > 3) { 
                await ctx.RespondAsync("Prefix cannot be longer than 3 characters.");
                return;
            }

            newPrefix = newPrefix.ToLower();
            await _guildSettingsService.SetPrefixAsync(ctx.Guild!.Id, newPrefix);

            await ctx.RespondAsync(
                MessageHelpers.GenericSuccessEmbed("Prefix updated", $"Prefix successfully updated to `{newPrefix}`.")
            );
        }
    }
}
