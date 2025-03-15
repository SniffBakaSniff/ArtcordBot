using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class GuildPresets 
{
    [Key]
    public ulong GuildId { get; set; }
    public string? Name { get; set; }
    public string? Channels { get; set; }
    public string? Members { get; set; }

}