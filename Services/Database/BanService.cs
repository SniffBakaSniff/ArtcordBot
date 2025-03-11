using Microsoft.EntityFrameworkCore;

namespace ArtcordBot.Services.Database
{
    public class BanService : IBanService
    {
        public async Task NewBanRecordAsync(BanRecord banRecord)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                await using var dbContext = new BotDbContext();
                {
                    banRecord.BanDate = DateTime.UtcNow;
                    dbContext.BanRecords.Add(banRecord);
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task RemoveBanRecordAsync(ulong guildId, ulong userId)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var banRecord = await dbContext.BanRecords.FirstOrDefaultAsync(b => b.GuildId == guildId && b.UserId == userId);
                    if (banRecord is not null)
                    {
                        dbContext.BanRecords.Remove(banRecord);
                        await dbContext.SaveChangesAsync();
                    }
                }
            });
        }
        
    }
}