using ArtcordBot.Services.Database;
using static ArtcordBot.Services.Database.PaginationService;

public interface IMessageSettingsService
{
    Task<string?> ManageMessageSettingAsync(ulong guildId, MessageType messageType, string? newMessage = null);
}

public interface IGuildSettingsService
{
    Task<ulong?> GetLogsChannelAsync(ulong guildId);
    Task<string> GetPrefixAsync(ulong guildId);
    Task SetPrefixAsync(ulong guildId, string prefix);
    Task<ulong?> GetMutedRoleAsync(ulong guildId);
    Task SetMutedRoleAsync(ulong guildId, ulong? mutedRoleId);
    Task<ulong?> GetWelcomeChannelAsync(ulong guildId);
    Task SetWelcomeChannelAsync(ulong guildId, ulong? welcomeChannelId);
    Task<ulong?> GetFarewellChannelAsync(ulong guildId);
    Task SetFarewellChannelAsync(ulong guildId, ulong? farewellChannelId);
    Task<string?> GetLockMessageAsync(ulong guildId);
    Task<string?> GetUnlockMessageAsync(ulong guildId);
}

public interface ITicketService
{
    Task<(ulong? TicketId, ulong? UserId)> GetTicketIdForChannelAsync(ulong channelId);
    Task<ulong?> GetTicketCountAsync(ulong guildId, ulong? userId = null);
    Task<bool> GetTicketClaimedStatusAsync(ulong ticketId);
    Task SetTicketClaimedStatusAsync(ulong ticketId, ulong userId, bool claimed);
    Task LogTicketMessageAsync(ulong ticketId, ulong userId, TicketMessageType messageType, string content);
    Task CloseTicketAsync(ulong ticketId);
    Task NewTicketRecordAsync(
        ulong guildId,
        ulong channelId,
        ulong userId,
        string reason,
        DateTime openedAt,
        DateTime? closedAt = null);
}

public interface IBanService
{
    Task NewBanRecordAsync(BanRecord banRecord);
    Task RemoveBanRecordAsync(ulong id, ulong targetUser);
}


public interface IPaginationService
{
    Task<PaginatedResult<T>> GetPaginatedResults<T>(IQueryable<T> query, int pageNumber = 1, int pageSize = 5);
}