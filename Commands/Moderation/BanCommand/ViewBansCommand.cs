using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.Commands.ContextChecks;
using System.ComponentModel;
using System.Threading.Channels;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup
    {
        [Command("viewbans")]
        [RequirePermissions(DiscordPermissions.BanMembers)]
        [Description("View all bans for the server.")]
        public async Task ViewBansAsync(CommandContext ctx, ulong? userId = null, int? banId = null)
        {
            if (userId.HasValue || banId.HasValue)
            {
                var result = await _banService.GetBanRecordsAsync(ctx.Guild!.Id, userId, banId, pageNumber: 1, pageSize: 5);
                if (result.Records == null || !result.Records.Any())
                {
                    await ctx.RespondAsync("No bans found for this server.");
                    return;
                }

                foreach (var ban in result.Records)
                {
                    var user = await ctx.Client.GetUserAsync(ban.UserId);
                    var detailedEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Ban Details",
                        Color = DiscordColor.Red,
                        Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl },
                        Timestamp = DateTime.UtcNow
                    }
                    .AddField("User", $"<@{ban.UserId}>", true)
                    .AddField("Moderator", $"<@{ban.ModeratorId}>", true)
                    .AddField("\u200B", "\u200B", true)
                    .AddField("Reason", ban.Reason ?? "No reason provided.", true)
                    .AddField("Notes", ban.InternalNotes ?? "None", true)
                    .WithFooter($"Ban ID: {ban.BanId} | Date: {ban.BanDate:yyyy-MM-dd HH:mm:ss}");

                    await ctx.RespondAsync(embed: detailedEmbed.Build());
                }
                return;
            }

            else
            {
                int pageNumber = 1;
                int pageSize = 5;
                var paginatedBans = await _banService.GetBanRecordsAsync(ctx.Guild!.Id, userId, banId, pageNumber, pageSize);

                if (paginatedBans.Records == null || !paginatedBans.Records.Any())
                {
                    await ctx.RespondAsync("No bans found for this server.");
                    return;
                }

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Ban List",
                    Color = DiscordColor.Red,
                    Timestamp = DateTime.UtcNow
                };

                foreach (var ban in paginatedBans.Records)
                {
                    embed.AddField(
                        $"**User ID:** {ban.UserId}",
                        $"**Moderator:** <@{ban.ModeratorId}>\n" +
                        $"**Reason:** {ban.Reason ?? "No reason provided."}\n" +
                        $"**Date:** {ban.BanDate:yyyy-MM-dd HH:mm:ss}\n" +
                        $"**Ban ID:** {ban.BanId}",
                        false);
                }

                embed.WithFooter($"Page {paginatedBans.CurrentPage}/{paginatedBans.TotalPages}");

                var messageBuilder = new DiscordMessageBuilder()
                    .AddEmbed(embed.Build());

                if (paginatedBans.TotalPages > 1 || paginatedBans.CurrentPage > 1)
                {
                    messageBuilder.AddComponents(
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "previous_banlist_page", "Previous", true),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "next_banlist_page", "Next", false)
                    );
                }

                await ctx.RespondAsync(messageBuilder);
            }
        }
    }
}
