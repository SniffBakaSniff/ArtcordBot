using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public class TicketRecords
{
    [Key]
    public ulong Id { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong UserId { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public ulong? ModeratorId { get; set; }
    public bool IsClaimed { get; set; } = false;
    public string? Reason { get; set; }
    public List<TicketMessages> Messages { get; set; } = new();
}

public class TicketMessages
{
    [Key]
    public ulong Id { get; set; }
    [ForeignKey(nameof(TicketRecords))]
    public ulong TicketId { get; set; }
    public ulong UserId { get; set; }
    public TicketMessageType MessageType { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TicketRecords? TicketRecords { get; set; }
}