using ArtcordBot.Helpers;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup
    {
        [Command("mute")]
        [System.ComponentModel.Description("Mutes a user")]
        public async Task MuteAsync(CommandContext ctx,
        [System.ComponentModel.Description("The user to mute.")] DiscordUser targetUser)
        {
            ulong? mutedRoleId = await _guildSettingsService.GetMutedRoleAsync(ctx.Guild!.Id);
            if (mutedRoleId == null)
            {
                await ctx.RespondAsync(MessageHelpers.GenericErrorEmbed("Muted role not set. Please configure the muted role with `/config setmutedrole`."));
                return;
            }

            var mutedRole = ctx.Guild.GetRole(mutedRoleId.Value);
            if (mutedRole is null)
            {
                await ctx.RespondAsync(MessageHelpers.GenericErrorEmbed("Muted role not found in the guild."));
                return;
            }

            var targetMember = await ctx.Guild.GetMemberAsync(targetUser.Id);
            await targetMember.GrantRoleAsync(mutedRole);

            await ctx.RespondAsync(MessageHelpers.GenericSuccessEmbed($"User Muted", $"{targetUser.Mention} has been muted."));
            await targetUser.SendMessageAsync(MessageHelpers.GenericErrorEmbed("You have been muted", $"You have been muted from {ctx.Guild.Name}."));
        }
    }
}
