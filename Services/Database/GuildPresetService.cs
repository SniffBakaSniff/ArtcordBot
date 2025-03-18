using Microsoft.EntityFrameworkCore;

namespace ArtcordBot.Services.Database
{
    public class GuildPresetService : IGuildPresetService
    {
        
        public async Task AddPresetAsync(ulong guildId, string name, string? channels, string? members)
        {
            await ExceptionHandler.HandleAsync(async () => 
            {
                using(var dbContext = new BotDbContext())
                {
                    var settings = await GuildPresetsAsync(dbContext, guildId, name);
                    settings.Name = name;
                    settings.Channels = channels;
                    settings.Members = members;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task<GuildPresets?> GetPresetAsync(ulong guildId, string name)
        {
            return await ExceptionHandler.HandleAsync(async() =>
            {
                using (var dbContext = new BotDbContext())
                {
                    return await dbContext.GuildPresets.FirstOrDefaultAsync(b => b.GuildId == guildId && b.Name == name);
                }
            });
        }

        public async Task<ulong[]?> GetPresetChannelsAsync(ulong guildId, string name)
        {
            return await ExceptionHandler.HandleAsync(async () => 
            {
                using (var dbContext = new BotDbContext())
                {
                    var presets = await dbContext.GuildPresets.FirstOrDefaultAsync(b => b.GuildId == guildId && b.Name == name);
                    ulong[] channelIds = presets!.Channels!
                        .Replace("<", "")
                        .Replace("#", "")
                        .Replace(">", "")
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(ulong.Parse)
                        .ToArray();

                    return channelIds;
                }
            });
        }

        public async Task<List<string?>> GetPresetNamesAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () => 
            {
                using (var dbContext = new BotDbContext())
                {
                    return await dbContext.GuildPresets
                        .Where(p => p.GuildId == guildId)
                        .Select(p => p.Name)
                        .ToListAsync();
                }
            }) ?? [];
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

        public async Task EditPresetAsync(ulong guildId, string name, string? newName, string? channels, string? members)
        {
            await ExceptionHandler.HandleAsync(async() =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var preset = await dbContext.GuildPresets.FirstOrDefaultAsync(b => b.GuildId == guildId && b.Name == name);

                    if (preset is not null)
                    {
                        if (newName is not null)
                        {
                            preset.Name = newName;
                        }

                        if (channels is not null)
                        {
                            preset.Channels = channels;
                        }

                        if (members is not null)
                        {
                            preset.Members = members;
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }
            });
        }

        private async Task<GuildPresets> GuildPresetsAsync(BotDbContext dbContext, ulong guildId, string name)
        {
            var settings = await dbContext.GuildPresets.FirstOrDefaultAsync(b => b.GuildId == guildId && b.Name == name);

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