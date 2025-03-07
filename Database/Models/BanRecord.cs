using System.ComponentModel.DataAnnotations;


public class BanRecord
{
    [Key]
    public int BanId { get; set; }

    public ulong GuildId { get; set; }

    public ulong UserId { get; set; }

    public ulong ModeratorId { get; set; }

    [MaxLength(250)]
    public string? Reason { get; set; }

    public string? ReferenceImagePath { get; set; }

    public ulong? ReferenceMessageId { get; set; }

    public DateTime BanDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpirationDate { get; set; }

    public string? AppealMessage { get; set; } //Temporary Placeholder till we have our own appeal system

    public AppealStatus? AppealStatus { get; set; }

    public DateTime? AppealDate { get; set; }

    [MaxLength(1000)]
    public string? InternalNotes { get; set; }
}