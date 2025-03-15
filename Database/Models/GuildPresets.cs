using System.ComponentModel.DataAnnotations;

public class GuildPresets 
{
    [Key]
    public int PresetId { get; set; }
    public ulong GuildId { get; set; }
    public string? Name { get; set; }
    public string? Channels { get; set; }
    public string? Members { get; set; }

}