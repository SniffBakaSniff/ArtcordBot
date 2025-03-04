using DSharpPlus.Entities;

namespace ArtcordBot.Helpers
{
    /// <summary>
    /// Presets for embed response messages
    /// </summary>
    public static class MessageHelpers
    {
        public static DiscordEmbed GenericSuccessEmbed(string title, string message) =>
            GenericEmbed(title, message, "#00ffff"); //I Like AQUA

        public static DiscordEmbed GenericErrorEmbed(string message, string title = "Error") =>
            GenericEmbed(title, message, "#ff0000");

        public static DiscordEmbed GenericEmbed(string title, string message, string color = "#5865f2") => new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithColor(new DiscordColor(color))
                .WithDescription(message)
                .WithTimestamp(DateTime.UtcNow)
                .Build();

        public static DiscordEmbed GenericUpdateEmbed(string title, string? extra, string color = "#00ffff") => new DiscordEmbedBuilder()
                .WithTitle(title)
                .WithColor(new DiscordColor(color))
                .WithTimestamp(DateTime.UtcNow)
                .AddField("Updated To:" , $"```{extra}```")
                .Build();
    };
}

