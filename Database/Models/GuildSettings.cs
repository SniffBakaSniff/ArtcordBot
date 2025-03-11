using System.ComponentModel.DataAnnotations;

public class GuildSettings
{
    [Key]
    public ulong GuildId { get; set; }
    public string Prefix { get; set; } = "!";
    public ulong? MutedRoleId { get; set; }
    public ulong? LogsChannelId { get; set; }
    public ulong? WelcomeChannelId { get; set; }
    public ulong? FarewellChannelId { get; set; }
}

public class GuildMessageSettings
{
    [Key]
    public ulong GuildId { get; set; }
    public string AppealMessage { get; set; } = "If you believe this ban was a mistake, please reach out to the moderation team for clarification.";
    public string WelcomeMessage { get; set; } = "Welcome to {server}! We hope you enjoy your stay {user}.";
    public string FarewellMessage { get; set; } = "We're sad to see you go {user}! If you have any feedback, feel free to share.";
    public string MutedNotificationMessage { get; set; } = "You have been muted in the {server} server .";
    public string TicketCreationMessage { get; set; } = "Your ticket has been created. Please wait for a moderator to assist you.";
    public string TicketClosureMessage { get; set; } = "Your ticket has been closed. Thank you for your patience!";
    public string ErrorMessage { get; set; } = "An error has occurred. Please try again later.";
}
