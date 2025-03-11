using DSharpPlus.Commands;
using DSharpPlus.Entities;
using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands.Processors.TextCommands;

[Command("ticket")]
[Description("Commands For the Ticket System")]
public partial class TicketCommandGroup
{
    private readonly ITicketService _ticketService;
    private readonly IGuildSettingsService _guildSettingsService;

    public TicketCommandGroup(ITicketService ticketService, IGuildSettingsService guildSettingsService)
    {
        _ticketService = ticketService;
        _guildSettingsService = guildSettingsService;
    }

    [Command("create")]
    public async Task TicketAsync(CommandContext ctx, [Description("Reason for the ticket.")] string reason)
    {
        if (ctx is TextCommandContext textContext)
        {
            
            await ctx.RespondAsync(new DiscordEmbedBuilder()
            {
                Description = "Please use `/ticket create` isntead.",
            }.Build());
            return;
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            await ctx.RespondAsync("Please provide a reason for your ticket.");
            return;
        }
        await ctx.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Creating ticket...").AsEphemeral());


        var ticketChannel = await CreateTicketChannelAsync(ctx, ctx.User, reason);

        // Log the ticket in the private log channel
        await LogTicketAsync(ctx, ctx.Guild!, ctx.User, reason);

        // Confirm to the user in a follow-up message
        var embed = new DiscordEmbedBuilder
        {
            Title = "Ticket Created",
            Description = $"Your ticket has been logged. Please check {ticketChannel.Mention}.",
            Color = DiscordColor.Green
        };

        await ctx.EditResponseAsync(new DiscordFollowupMessageBuilder().WithContent("Ticket Created!")
            .AddEmbed(embed)
            .AsEphemeral());
    }

    private async Task<DiscordChannel> CreateTicketChannelAsync(CommandContext ctx, DiscordUser user, string reason)
    {
        ulong ticketNumber = (ulong)await _ticketService.GetTicketCountAsync(guildId: ctx.Guild!.Id, userId: user.Id);
        string ticketChannelName = $"{user.Username}-ticket-{ticketNumber}";

        var ticketChannel = await ctx.Guild!.CreateChannelAsync(
            ticketChannelName,
            DiscordChannelType.Text
        );

        await ticketChannel.AddOverwriteAsync(ctx.Guild.EveryoneRole, deny: DiscordPermissions.AccessChannels);
        await ticketChannel.AddOverwriteAsync((DiscordMember)user, allow: DiscordPermissions.AccessChannels | DiscordPermissions.SendMessages | DiscordPermissions.ReadMessageHistory);
        
        var modRole = ctx.Guild.Roles.Values.FirstOrDefault(r => r.Permissions.HasPermission(DiscordPermissions.ManageMessages));
        if (modRole is not null)
        {
            await ticketChannel.AddOverwriteAsync(modRole, allow: DiscordPermissions.AccessChannels | DiscordPermissions.SendMessages | DiscordPermissions.ReadMessageHistory);
        }

        // Add message with buttons
        var ticketEmbed = new DiscordEmbedBuilder
        {
            Title = "Support Ticket",
            Color = DiscordColor.Blurple,
            Description = $"**User:** {user.Mention}\n**Reason:** {reason}\n\n" +
                            "**A staff member will assist you shortly.**"
        };

        await ticketChannel.SendMessageAsync(new DiscordMessageBuilder()
            .AddEmbed(ticketEmbed)
            .AddComponents(
                new DiscordButtonComponent(DiscordButtonStyle.Primary, "claim_ticket", "Claim Ticket"),
                new DiscordButtonComponent(DiscordButtonStyle.Danger, "close_ticket", "Close Ticket")
            )
        );

        // Log the ticket record in the database
        await _ticketService.NewTicketRecordAsync(
            guildId: ctx.Guild.Id,
            channelId: ticketChannel.Id,
            userId: user.Id,
            reason: reason,
            openedAt: DateTime.UtcNow
        );

        return ticketChannel;
    }

    private async Task LogTicketAsync(CommandContext ctx, DiscordGuild guild, DiscordUser user, string reason)
    {
        ulong? LogsChannelId = await _guildSettingsService.GetLogsChannelAsync(ctx.Guild!.Id);

        if (LogsChannelId is null)
        {
            return;
        }

        var logChannel = await guild.GetChannelAsync((ulong)LogsChannelId);

        var logEmbed = new DiscordEmbedBuilder
        {
            Title = "New Support Ticket",
            Color = DiscordColor.Blurple,
            Description = $"**User:** {user.Username}#{user.Discriminator}\n" +
                            $"**User ID:** {user.Id}\n" +
                            $"**Reason:** {reason}\n" +
                            $"**Created at:** {Formatter.Timestamp(DateTime.UtcNow)}",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = user.AvatarUrl
            }
        };

        await logChannel.SendMessageAsync(embed: logEmbed);
    }
}

