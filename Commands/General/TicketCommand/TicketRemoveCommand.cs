using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;

public partial class TicketCommandGroup
{

    [Command("remove")]
    [Description("Removes a user from a ticket.")]
    public async Task RemoveTicketUserAsync(CommandContext ctx,
    [Description("The user to remove from the ticket.")] DiscordMember? targetUser = null,
    [Description("The role to remove from the ticket.")] DiscordRole? targetRole = null,
    [Description("The ticket to remove the user or role from.")] DiscordChannel? targetChannel = null, 
    [Description("The reason for the removal.")] string? reason = null)
    {
        if (ctx is TextCommandContext textContext)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder()
            {
                Description = "Please use `/ticket remove` instead.",
            }.Build());
            return;
        }

        if (targetUser is null && targetRole is null)
        {
            await ctx.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Please provide a user or role to remove from the ticket.").AsEphemeral());
        }

        if (targetChannel is null)
        {
            var (ticketId, _) = await _ticketService.GetTicketIdForChannelAsync(ctx.Channel.Id);

            if (ticketId is null)
            {
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder().WithContent("Please provide a ticket to remove the user or role from.").AsEphemeral());
            }
        }

        var msg = new DiscordEmbedBuilder()
            .WithDescription($"**User:** {targetUser?.Mention ?? targetRole?.Mention} has been removed from the ticket {targetChannel!.Mention}.\n**Reason:** {reason}\n\n")
            .WithColor(DiscordColor.Cyan)
            .WithFooter("Removed User from Ticket",ctx.Client.CurrentUser.AvatarUrl)
            .WithTimestamp(DateTime.UtcNow);

        if (targetChannel is not null)
        {
            await ctx.RespondAsync(new DiscordInteractionResponseBuilder().AddEmbed(msg).AsEphemeral());
            await targetChannel.SendMessageAsync(msg.Build());
        }
        
        targetChannel ??= ctx.Channel;

        if (targetUser is not null)
            await targetChannel.DeleteOverwriteAsync(targetUser);
            await ctx.RespondAsync(msg.Build());

        if (targetRole is not null)
            await targetChannel.DeleteOverwriteAsync(targetRole);
    }

}