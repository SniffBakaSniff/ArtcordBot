using ArtcordBot.Helpers;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace ArtcordBot.Features
{
    [Command("echo")]
    public class EchoCommand
    {
        [Command("plain")]
        [System.ComponentModel.Description("Send a plain text message as the bot")]
        public static ValueTask ExecuteAsync(CommandContext context, 
            [System.ComponentModel.Description("The message to be sent")] string message,
            [System.ComponentModel.Description("The channel the message should be sent (current channel by default)")] DiscordChannel? channel = null)
        {
            channel ??= context.Channel;

            channel.SendMessageAsync(message);

            return context.RespondAsync(
                MessageHelpers.GenericSuccessEmbed("Message sent", $"Message successfully sent to {channel.Mention}")
                );
        }

        [Command("embed")]
        [System.ComponentModel.Description("Send an embed message as the bot")]
        public static ValueTask ExecuteAsync(CommandContext context,
            [System.ComponentModel.Description("The description of the embed")] string message,
            [System.ComponentModel.Description("The channel the embed should be sent (current channel by default)")] DiscordChannel? channel = null,
            [System.ComponentModel.Description("The title above the description of the embed")] string? title = null,
            [System.ComponentModel.Description("The footer below the description of the embed")] string? footer = null,
            [System.ComponentModel.Description("The author of the embed shown above the title")] DiscordUser? author = null,
            [System.ComponentModel.Description("The hex color code of the embed")] string color = "#5865f2",
            [System.ComponentModel.Description("Whether to include the current timestamp or not")] bool withTimestamp = false)
        {
            DiscordEmbedBuilder embedBuilder = new();

            embedBuilder
                .WithDescription(message)
                .WithFooter(footer)
                .WithColor(new DiscordColor(color));

            if (title != null)
                embedBuilder.WithTitle(title);

            if (footer != null)
                embedBuilder.WithFooter(footer);

            if (author != null)
                embedBuilder.WithAuthor(author.GlobalName, null, author.AvatarUrl);

            if (withTimestamp)
                embedBuilder.WithTimestamp(DateTime.UtcNow);

            DiscordEmbed embed = embedBuilder.Build();

            channel ??= context.Channel;

            channel.SendMessageAsync(embed);

            return context.RespondAsync(
                MessageHelpers.GenericSuccessEmbed("Embed sent", $"Embed successfully sent to {channel.Mention}")
                );
        }
    }
}
