using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.Commands.ContextChecks;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup
    {
        

        [Command("Unban")]
        [RequirePermissions(DiscordPermissions.BanMembers)]
        public async Task BanAsync(CommandContext ctx, 
        [System.ComponentModel.Description("The user to unban.")]ulong targetUser, 
        [System.ComponentModel.Description("The reason for the unban.")] string? reason = null)
        {

            await _banService.RemoveBanRecordAsync(ctx.Guild!.Id, targetUser);

            await ctx.Guild!.UnbanMemberAsync(targetUser, reason);

            var embed = new DiscordEmbedBuilder
            {
                Title = "User Unbanned",
                Color = DiscordColor.Cyan,
                Description = $"**User:** {targetUser}\n" +
                              $"**Reason:** {reason}\n" +
                              $"**Moderator:** {ctx.User.Username}",
                Timestamp = DateTime.UtcNow
            };

            await ctx.RespondAsync(embed: embed.Build());
        }
    }
}
