using Microsoft.EntityFrameworkCore;

namespace ArtcordBot.Services.Database
{
    public class TicketService : ITicketService
    {
        public async Task<(ulong? TicketId, ulong? UserId)> GetTicketIdForChannelAsync(ulong channelId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var ticket = await dbContext.TicketRecords
                        .FirstOrDefaultAsync(t => t.ChannelId == channelId);

                    return (ticket?.Id, ticket?.UserId);
                }
            });
        }

        public async Task<ulong?> GetTicketCountAsync(ulong guildId, ulong? userId = null)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    return (ulong?)await dbContext.TicketRecords
                        .Where(t => t.GuildId == guildId && (userId == null || t.UserId == userId))
                        .CountAsync();
                }
            });
        }

        public async Task<bool> GetTicketClaimedStatusAsync(ulong ticketId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    return await dbContext.TicketRecords
                        .AnyAsync(t => t.Id == ticketId && t.IsClaimed);
                }
            });
        }

        public async Task SetTicketClaimedStatusAsync(ulong ticketId, ulong userId, bool isClaimed)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var ticket = await dbContext.TicketRecords
                        .FirstOrDefaultAsync(t => t.Id == ticketId);
                    ticket!.IsClaimed = isClaimed;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task LogTicketMessageAsync(ulong ticketId, ulong userId, TicketMessageType messageType, string content)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var newMessage = new TicketMessages
                    {
                        TicketId = ticketId,
                        UserId = userId,
                        MessageType = messageType,
                        Content = content
                    };

                    dbContext.TicketMessages.Add(newMessage);
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task CloseTicketAsync(ulong ticketId)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var ticket = await dbContext.TicketRecords.FindAsync(ticketId);
                    ticket!.ClosedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task NewTicketRecordAsync(
            ulong guildId,
            ulong channelId,
            ulong userId,
            string reason,
            DateTime openedAt,
            DateTime? closedAt = null)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var newTicketRecord = new TicketRecord
                    {
                        GuildId = guildId,
                        ChannelId = channelId,
                        UserId = userId,
                        OpenedAt = DateTime.UtcNow,
                        ClosedAt = closedAt,
                        Reason = reason
                    };

                    dbContext.TicketRecords.Add(newTicketRecord);
                    await dbContext.SaveChangesAsync();
                }
            });
        }
    }
}