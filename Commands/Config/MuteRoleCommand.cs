using DSharpPlus.Commands;
using DSharpPlus.Entities;
using ArtcordBot.Helpers;


namespace ArtcordBot.Features.ConfigCommands
{
    public partial class ConfigCommandsGroup
    {
        [Command("mutedrole")]
        [System.ComponentModel.Description("Set the muted role")]
        public async Task SetMutedRoleAsync(CommandContext ctx, DiscordRole mutedRoleId)
        {
            if (mutedRoleId is null)
            {
                await ctx.RespondAsync(MessageHelpers.GenericErrorEmbed("You must provide a role."));
                return;
            }

            await _guildSettingsService.SetMutedRoleAsync(ctx.Guild!.Id, mutedRoleId.Id);
            await ctx.RespondAsync(
                MessageHelpers.GenericSuccessEmbed("Muted Role Updated", $"Muted Role successfully updated to {mutedRoleId.Mention}.")
            );
        }
    }
}
