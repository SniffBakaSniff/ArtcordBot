using Microsoft.EntityFrameworkCore;

namespace ArtcordBot.Services.Database
{
    public class GuildPresetService : IGuildPresetService
    {
        
        public async Task SetPresetChannelsAsync(ulong guildId, string name, string channels)
        {
            await ExceptionHandler.HandleAsync(async () => 
            {
                using(var dbContext = new BotDbContext())
                {
                    var settings = await GuildPresetsAsync(dbContext, guildId);
                    settings.Name = name;
                    settings.Channels = channels;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task<string?> GetPresetChannelsAsync(ulong guildId, string name)
        {
            return await ExceptionHandler.HandleAsync(async () => 
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildPresets.FirstOrDefaultAsync(b => b.GuildId == guildId && b.Name == name);
                    return settings?.Channels;
                }
            });
        }

        public async Task<string?> GetPresetNamesAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () => 
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildPresets.FindAsync(guildId);
                    return settings?.Name;
                }
            });
        }

        public async Task RemovePresetChannelsAsync(ulong guildId, string name)
        {
            await ExceptionHandler.HandleAsync(async() => 
            {
                using (var dbContext = new BotDbContext())
                {
                    var preset = await dbContext.GuildPresets.FirstOrDefaultAsync(b => b.GuildId == guildId && b.Name == name);
                    if (preset is not null)
                    {
                        dbContext.GuildPresets.Remove(preset);
                        await dbContext.SaveChangesAsync();
                    }
                }
            });
        }

        private async Task<GuildPresets> GuildPresetsAsync(BotDbContext dbContext, ulong guildId)
        {
            var settings = await dbContext.GuildPresets.FindAsync(guildId);

            if (settings is null)
            {
                settings = new GuildPresets { GuildId = guildId };
                dbContext.GuildPresets.Add(settings);
            }
            settings.GuildId = guildId;

            return settings;
        }
    }
}