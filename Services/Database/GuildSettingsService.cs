namespace ArtcordBot.Services.Database
{
    public class GuildSettingsService : IGuildSettingsService
    {
        public async Task<ulong?> GetLogsChannelAsync(ulong guildId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildSettings.FindAsync(guildId);
                    return settings?.LogsChannelId;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetLogsChannelAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetPrefixAsync(ulong guildId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildSettings.FindAsync(guildId);
                    return settings?.Prefix ?? "!";
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetPrefixAsync: {ex.Message}");
                return "!";
            }
        }

        public async Task SetPrefixAsync(ulong guildId, string prefix)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings.Prefix = prefix;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SetPrefixAsync: {ex.Message}");
            }
        }

        public async Task<ulong?> GetMutedRoleAsync(ulong guildId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildSettings.FindAsync(guildId);
                    return settings?.MutedRoleId;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetMutedRoleAsync: {ex.Message}");
                return null;
            }
        }

        public async Task SetMutedRoleAsync(ulong guildId, ulong? mutedRoleId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings.MutedRoleId = mutedRoleId;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SetMutedRoleAsync: {ex.Message}");
            }
        }

        public async Task<ulong?> GetWelcomeChannelAsync(ulong guildId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    return settings?.WelcomeChannelId;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetWelcomeChannelAsync: {ex.Message}");
                return null;
            }
        }

        public async Task SetWelcomeChannelAsync(ulong guildId, ulong? welcomeChannelId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings!.WelcomeChannelId = welcomeChannelId;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SetWelcomeChannelAsync: {ex.Message}");
            }
        }

        public async Task<ulong?> GetFarewellChannelAsync(ulong guildId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    return settings?.FarewellChannelId;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetFarewellChannelAsync: {ex.Message}");
                return null;
            }
        }

        public async Task SetFarewellChannelAsync(ulong guildId, ulong? farewellChannelId)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings!.FarewellChannelId = farewellChannelId;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SetFarewellChannelAsync: {ex.Message}");
            }
        }

        private async Task<GuildSettings> GuildSettingsAsync(BotDbContext dbContext, ulong guildId)
        {
            var settings = await dbContext.GuildSettings.FindAsync(guildId);

            if (settings == null)
            {
                settings = new GuildSettings { GuildId = guildId };
                dbContext.GuildSettings.Add(settings);
            }

            return settings;
        }
    }
}