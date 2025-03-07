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
                    if (banRecord != null)
                    {
                        dbContext.BanRecords.Remove(banRecord);
                        await dbContext.SaveChangesAsync();
                    }
                }
            });
        }

        public async Task<PaginatedResult<BanRecord>> GetBanRecordsAsync(ulong guildId, ulong? userId = null, int? banId = null, int pageNumber = 1, int pageSize = 5)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var query = dbContext.BanRecords.Where(b => b.GuildId == guildId && 
                        (banId == null || b.BanId == banId) && 
                        (userId == null || b.UserId == userId));

                    var totalRecords = await query.CountAsync();
                    var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                    var records = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

                    return new PaginatedResult<BanRecord>
                    {
                        Records = records,
                        TotalRecords = totalRecords,
                        TotalPages = totalPages,
                        CurrentPage = pageNumber,
                        PageSize = pageSize
                    };
                }
            });
        }

        public class PaginatedResult<T>
        {
            public required List<T> Records { get; set; }
            public int TotalRecords { get; set; }
            public int TotalPages { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
        }
        
    }
}