namespace ArtcordBot.Services.Database
{
    public class BanService : IBanService
    {
        public async Task NewBanRecordAsync(
            ulong guildId,
            ulong userId,
            ulong moderatorId,
            string? reason = null,
            string? referenceImagePath = null,
            ulong? referenceMessageId = null,
            DateTime? expirationDate = null,
            AppealStatus? appealStatus = null,
            DateTime? appealDate = null,
            string? internalNotes = null)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var newBanRecord = new BanRecords
                    {
                        GuildId = guildId,
                        UserId = userId,
                        ModeratorId = moderatorId,
                        Reason = reason,
                        ReferenceImagePath = referenceImagePath,
                        ReferenceMessageId = referenceMessageId,
                        BanDate = DateTime.UtcNow,
                        ExpirationDate = expirationDate,
                        AppealStatus = appealStatus,
                        AppealDate = appealDate,
                        InternalNotes = internalNotes
                    };

                    dbContext.BanRecords.Add(newBanRecord);
                    await dbContext.SaveChangesAsync();
                }
            });
        }
    }
}