using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;

public partial class TicketCommandGroup
{

    [Command("add")]
    [Description("Adds a user to a ticket.")]
    public async Task AddTicketUserAsync(CommandContext ctx,
    [Description("The user to add to the ticket.")] DiscordMember? targetUser = null,
    [Description("The role to add to the ticket.")] DiscordRole? targetRole = null,
    [Description("The ticket to add the user or role to.")] DiscordChannel? targetChannel = null, 
    [Description("The reason for the addition.")] string? reason = null)
    {
        if (ctx is TextCommandContext textContext)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder()
            {
                Description = "Please use `/ticket add` instead.",
            }.Build());
            return;
        }

        if (targetUser is null && targetRole is null)
        {
            await ctx.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Please provide a user or role to add to the ticket.").AsEphemeral());
        }

        if (targetChannel is null)
        {
            var (ticketId, _) = await _ticketService.GetTicketIdForChannelAsync(ctx.Channel.Id);

            if (ticketId is null)
            {
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Please provide a ticket to add the user or role to.").AsEphemeral());
            }
        }

        var msg = new DiscordEmbedBuilder()
            .WithDescription($"**User:** {targetUser?.Mention ?? targetRole?.Mention} has been added to the ticket {targetChannel!.Mention}.\n**Reason:** {reason}\n\n")
            .WithColor(DiscordColor.Cyan)
            .WithFooter("Added User to Ticket",ctx.Client.CurrentUser.AvatarUrl)
            .WithTimestamp(DateTime.UtcNow);

        if (targetChannel is not null)
        {
            await ctx.RespondAsync(new DiscordInteractionResponseBuilder().AddEmbed(msg).AsEphemeral());
            await targetChannel.SendMessageAsync(msg.Build());
        }
        
        targetChannel ??= ctx.Channel;

        if (targetUser is not null)
            await targetChannel.AddOverwriteAsync(targetUser!, allow: DiscordPermissions.SendMessages | DiscordPermissions.ReadMessageHistory);
            await ctx.RespondAsync(msg.Build());

        if (targetRole is not null)
            await targetChannel.AddOverwriteAsync(targetRole!, allow: DiscordPermissions.SendMessages | DiscordPermissions.ReadMessageHistory);
    }

}