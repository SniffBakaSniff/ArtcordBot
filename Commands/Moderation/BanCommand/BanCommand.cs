using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.Commands.ContextChecks;
using System.ComponentModel;
using DSharpPlus;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup
    {
        private readonly HttpClient _httpClient;
        
        [Command("ban")]
        [RequirePermissions(DiscordPermissions.BanMembers)] 
        // Placeholder till we have a custom permission handler. Whenever we get a custom permission handler we can just handle permissions on a per group basis.
        public async Task BanAsync(CommandContext ctx,
        [Description("The user to ban.")]DiscordUser targetUser, 
        [Description("The reason for the ban.")] string? reason = null, 
        [Description("The attachment of the reference image.")] DiscordAttachment? attachment = null, 
        [Description("The ID of the reference message.")] ulong? referenceMessageId = null, 
        [Description("Additional notes about the ban.")]string? internalNotes = null, 
        [Description("Choose the timeframe for deleting user messages.")] MessageDeletionTimeframe deleteTimeframe = MessageDeletionTimeframe.None)
        {

            var guildIconUrl = ctx.Guild!.IconUrl;

            // Attachment processing
            string? referenceImagePath = null;
            if (attachment != null)
            {
                // Download and save the image
                string fileName = $"ban_reference_{ctx.Guild!.Id}_{targetUser.Id}_{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss.fff")}.png";
                string filePath = Path.Combine("Images", fileName);
                
                Directory.CreateDirectory("Images");
                
                using (var imageStream = await _httpClient.GetStreamAsync(attachment.Url))
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await imageStream.CopyToAsync(fileStream);
                }

                referenceImagePath = filePath;
            }

            // Create the ban record in the database
            await _banService.NewBanRecordAsync(new BanRecord
            {
                GuildId = ctx.Guild!.Id,
                UserId = targetUser.Id,
                ModeratorId = ctx.User.Id,
                Reason = reason,
                ReferenceImagePath = referenceImagePath,
                ReferenceMessageId = referenceMessageId,
                InternalNotes = internalNotes
            });

            reason ??= "No reason provided.";


            DateTime adjustedTime = DateTime.UtcNow.AddHours(-(int)deleteTimeframe);

            string deletionTimeframeInfo = deleteTimeframe == MessageDeletionTimeframe.None
                ? "No messages were deleted."
                : $"Messages from {Formatter.Timestamp(adjustedTime, TimestampFormat.ShortDateTime)} to {Formatter.Timestamp(DateTime.UtcNow, TimestampFormat.ShortDateTime)} were deleted.";

            var embed = new DiscordEmbedBuilder
            {
                Title = "User Banned",
                Color = DiscordColor.Red,
                Description = $"**User:** {targetUser.Username}\n" +
                              $"**Moderator:** {ctx.User.Username}\n" +
                              $"**Deleted Messages:** {deletionTimeframeInfo}",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = targetUser.AvatarUrl
                },
                Timestamp = DateTime.UtcNow
            }.AddField("Reason", $"```\n{reason}\n```");

            var dmEmbed = new DiscordEmbedBuilder
            {
                Title = "You Have Been Banned",
                Color = DiscordColor.Red,
                Description = $"**Server:** {ctx.Guild.Name}\n" +
                            $"**Appeals:** If you believe this ban was a mistake, please reach out to the moderation team for clarification.",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = guildIconUrl
                },
                Timestamp = DateTime.UtcNow
            }.AddField("Reason", $"```\n{reason}\n```");

            await targetUser.SendMessageAsync(new DiscordMessageBuilder()
                .AddEmbeds([dmEmbed.Build()])
                .AddComponents(new DiscordButtonComponent(DiscordButtonStyle.Primary, "appeal_ban", "Appeal Ban"))
            );

            await ctx.RespondAsync(embed: embed.Build());
            //Commenting out for testing purposes (dont actually wanna ban people yet)
            //await ctx.Guild.BanMemberAsync(targetUser, TimeSpan.FromHours((int)deleteTimeframe), reason);
        }
    }
}