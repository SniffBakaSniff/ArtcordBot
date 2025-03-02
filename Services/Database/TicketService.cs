using Microsoft.EntityFrameworkCore;

namespace ArtcordBot.Services.Database
{
    public class TicketService : ITicketService
    {
        public async Task<(ulong? TicketId, ulong? UserId)> GetTicketIdForChannelAsync(ulong channelId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var ticket = await dbContext.TicketRecords
                        .FirstOrDefaultAsync(t => t.ChannelId == channelId);

                    return (ticket?.Id, ticket?.UserId);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetTicketIdForChannelAsync: {ex.Message}");
                return (null, null);
            }
        }

        public async Task<ulong?> GetTicketCountAsync(ulong guildId, ulong? userId = null)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    return (ulong?)await dbContext.TicketRecords
                        .Where(t => t.GuildId == guildId && (userId == null || t.UserId == userId))
                        .CountAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetTicketCountAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> GetTicketClaimedStatusAsync(ulong ticketId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    return await dbContext.TicketRecords
                        .AnyAsync(t => t.Id == ticketId && t.IsClaimed);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetTicketClaimedStatusAsync: {ex.Message}");
                return false;
            }
        }

        public async Task SetTicketClaimedStatusAsync(ulong ticketId, ulong userId, bool isClaimed)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var ticket = await dbContext.TicketRecords
                        .FirstOrDefaultAsync(t => t.Id == ticketId);
                    ticket!.IsClaimed = isClaimed;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SetTicketClaimedStatusAsync: {ex.Message}");
            }
        }

        public async Task LogTicketMessageAsync(ulong ticketId, ulong userId, TicketMessageType messageType, string content)
        {
            try
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
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in LogTicketMessageAsync: {ex.Message}");
            }
        }

        public async Task CloseTicketAsync(ulong ticketId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var ticket = await dbContext.TicketRecords.FindAsync(ticketId);
                    ticket!.ClosedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in CloseTicketAsync: {ex.Message}");
            }
        }

        public async Task NewTicketRecordAsync(
            ulong guildId,
            ulong channelId,
            ulong userId,
            string reason,
            DateTime openedAt,
            DateTime? closedAt = null)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var newTicketRecord = new TicketRecords
                    {
                        GuildId = guildId,
                        ChannelId = channelId,
                        UserId = userId,
                        OpenedAt = openedAt = DateTime.UtcNow,
                        ClosedAt = closedAt,
                        Reason = reason
                    };

                    dbContext.TicketRecords.Add(newTicketRecord);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in NewTicketRecordAsync: {ex.Message}");
            }
        }
    }
}