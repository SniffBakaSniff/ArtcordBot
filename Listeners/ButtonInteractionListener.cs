using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace ArtcordBot.Listeners
{
    public class ButtonInteractionListener
    {

        private readonly ITicketService _ticketService;
        private readonly IBanService _banService;

        public ButtonInteractionListener(ITicketService ticketService, IBanService banService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _banService = banService ?? throw new ArgumentNullException(nameof(banService));
        }

        public async Task HandleButtonInteraction(DiscordClient client, ComponentInteractionCreatedEventArgs e)
        {
            switch (e.Id)
            {
                case "claim_ticket":
                    var (ticketId, userId) = await _ticketService.GetTicketIdForChannelAsync(e.Channel.Id);

                    if (!ticketId.HasValue)
                    {
                        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("Ticket not found.").AsEphemeral(true));
                        return;
                    }

                    bool isClaimed = await _ticketService.GetTicketClaimedStatusAsync(ticketId.Value);

                    if (isClaimed)
                    {
                        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("This ticket has already been claimed.").AsEphemeral(true));
                        return;
                    }

                    var member = await e.Guild.GetMemberAsync(e.User.Id);
                    if (!member.Permissions.HasPermission(DiscordPermissions.ManageMessages))
                                                                                            
                    {
                        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent("You do not have permission to claim this ticket.").AsEphemeral(true));
                        return;
                    }

                    var originalMessage = await e.Channel.GetMessageAsync(e.Message.Id);
                    
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = originalMessage.Embeds[0].Title,
                        Description = originalMessage.Embeds[0].Description + $"\n\n**This ticket has been claimed by:** {e.User.Mention}",
                        Color = originalMessage.Embeds[0].Color
                    };

                    await originalMessage.ModifyAsync(msg =>
                    {
                        msg.RemoveEmbedAt(0);
                        msg.AddEmbed(embed.Build());
                    });

                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
                    break;

                case "close_ticket":
                    var closeEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Close Ticket",
                        Description = "Are you sure you want to close this ticket?",
                        Color = DiscordColor.Red
                    }.WithFooter("This action cannot be undone.");

                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder()
                            .AddEmbed(closeEmbed.Build())
                            .AddComponents(
                                new DiscordButtonComponent(DiscordButtonStyle.Danger, "close_ticket_confirmation", "Confirm"),
                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "close_ticket_cancellation", "Cancel")
                            )
                            .AsEphemeral(true));
                    break;

                case "appeal_ban":
                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                        new DiscordInteractionResponseBuilder().WithContent("Appeal sent.")); // Placeholder for actual appeal logic
                    break;

                case "close_ticket_confirmation":
                    await e.Channel.DeleteAsync();
                    break;

                case "close_ticket_cancellation":
                    var cancelEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Close Ticket",
                        Description = "Ticket closure has been canceled.",
                        Color = DiscordColor.Green
                    };

                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
                        new DiscordInteractionResponseBuilder()
                            .AddEmbed(cancelEmbed.Build())
                            .AddComponents(
                                new DiscordButtonComponent(DiscordButtonStyle.Danger, "close_ticket_confirmation", "Confirm", disabled: true),
                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "close_ticket_cancellation", "Cancel", disabled: true)
                            )
                            .AsEphemeral(true));
                    break;

                case "next_banlist_page":
                {
                    var originalEmbed = e.Message.Embeds.FirstOrDefault();
                    int currentPage = 1, totalPages = 1;
                    if (originalEmbed != null && originalEmbed.Footer != null && !string.IsNullOrEmpty(originalEmbed.Footer.Text))
                    {
                        var parts = originalEmbed.Footer.Text.Replace("Page ", "").Split('/');
                        if (parts.Length == 2)
                        {
                            int.TryParse(parts[0].Trim(), out currentPage);
                            int.TryParse(parts[1].Trim(), out totalPages);
                        }
                    }

                    int newPage = currentPage + 1;
                    if (newPage > totalPages)
                        newPage = totalPages;

                    var paginatedResult = await _banService.GetBanRecordsAsync(e.Guild.Id, null, null, newPage, 5);

                    var newPageEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Ban List",
                        Color = DiscordColor.Red,
                        Timestamp = DateTime.UtcNow,
                    };

                    foreach (var ban in paginatedResult.Records)
                    {
                        newPageEmbed.AddField(
                            $"**User ID:** {ban.UserId}",
                            $"**Moderator:** <@{ban.ModeratorId}>\n" +
                            $"**Reason:** {ban.Reason ?? "No reason provided."}\n" +
                            $"**Date:** {ban.BanDate:yyyy-MM-dd HH:mm:ss}\n" +
                            $"**Ban ID:** {ban.BanId}",
                            false);
                    }

                    newPageEmbed.WithFooter($"Page {paginatedResult.CurrentPage}/{paginatedResult.TotalPages}");

                    bool disablePrevious = paginatedResult.CurrentPage <= 1;
                    bool disableNext = paginatedResult.CurrentPage >= paginatedResult.TotalPages;

                    var responseBuilder = new DiscordInteractionResponseBuilder()
                        .AddEmbed(newPageEmbed.Build())
                        .AddComponents(
                            new DiscordButtonComponent(DiscordButtonStyle.Primary, "previous_banlist_page", "Previous", disablePrevious),
                            new DiscordButtonComponent(DiscordButtonStyle.Primary, "next_banlist_page", "Next", disableNext)
                        );

                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, responseBuilder);
                    break;
                }

                case "previous_banlist_page":
                {
                    var originalEmbed = e.Message.Embeds.FirstOrDefault();
                    int currentPage = 1, totalPages = 1;
                    if (originalEmbed != null && originalEmbed.Footer != null && !string.IsNullOrEmpty(originalEmbed.Footer.Text))
                    {
                        var parts = originalEmbed.Footer.Text.Replace("Page ", "").Split('/');
                        if (parts.Length == 2)
                        {
                            int.TryParse(parts[0].Trim(), out currentPage);
                            int.TryParse(parts[1].Trim(), out totalPages);
                        }
                    }

                    int newPage = currentPage - 1;
                    if (newPage < 1)
                        newPage = 1;

                    var paginatedResult = await _banService.GetBanRecordsAsync(e.Guild.Id, null, null, newPage, 5);

                    var previousPageEmbed = new DiscordEmbedBuilder
                    {
                        Title = "Ban List",
                        Color = DiscordColor.Red,
                        Timestamp = DateTime.UtcNow,
                    };

                    foreach (var ban in paginatedResult.Records)
                    {
                        previousPageEmbed.AddField(
                            $"**User ID:** {ban.UserId}",
                            $"**Moderator:** <@{ban.ModeratorId}>\n" +
                            $"**Reason:** {ban.Reason ?? "No reason provided."}\n" +
                            $"**Date:** {ban.BanDate:yyyy-MM-dd HH:mm:ss}\n" +
                            $"**Ban ID:** {ban.BanId}",
                            false);
                    }

                    previousPageEmbed.WithFooter($"Page {paginatedResult.CurrentPage}/{paginatedResult.TotalPages}");

                    bool disablePrevious = paginatedResult.CurrentPage <= 1;
                    bool disableNext = paginatedResult.CurrentPage >= paginatedResult.TotalPages;

                    var responseBuilder = new DiscordInteractionResponseBuilder()
                        .AddEmbed(previousPageEmbed.Build())
                        .AddComponents(
                            new DiscordButtonComponent(DiscordButtonStyle.Primary, "previous_banlist_page", "Previous", disablePrevious),
                            new DiscordButtonComponent(DiscordButtonStyle.Primary, "next_banlist_page", "Next", disableNext)
                        );

                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, responseBuilder);
                    break;
                }

                default:
                    break;
            }
        }
    }
}
