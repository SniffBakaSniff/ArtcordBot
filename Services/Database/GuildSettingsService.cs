namespace ArtcordBot.Services.Database
{
    public class GuildSettingsService : IGuildSettingsService
    {
        public async Task<ulong?> GetLogsChannelAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildSettings.FindAsync(guildId);
                    return settings?.LogsChannelId;
                }
            });
        }

        public async Task<string> GetPrefixAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildSettings.FindAsync(guildId);
                    return settings?.Prefix ?? "!";
                }
            });
        }

        public async Task SetPrefixAsync(ulong guildId, string prefix)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings.Prefix = prefix;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task<ulong?> GetMutedRoleAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await dbContext.GuildSettings.FindAsync(guildId);
                    return settings?.MutedRoleId;
                }
            });
        }

        public async Task SetMutedRoleAsync(ulong guildId, ulong? mutedRoleId)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings.MutedRoleId = mutedRoleId;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task<ulong?> GetWelcomeChannelAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    return settings?.WelcomeChannelId;
                }
            });
        }

        public async Task SetWelcomeChannelAsync(ulong guildId, ulong? welcomeChannelId)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings!.WelcomeChannelId = welcomeChannelId;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task<ulong?> GetFarewellChannelAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    return settings?.FarewellChannelId;
                }
            });
        }

        public async Task SetFarewellChannelAsync(ulong guildId, ulong? farewellChannelId)
        {
            await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildSettingsAsync(dbContext, guildId);
                    settings!.FarewellChannelId = farewellChannelId;
                    await dbContext.SaveChangesAsync();
                }
            });
        }

        public async Task<string?> GetLockMessageAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildMessageSettingsAsync(dbContext, guildId);
                    return settings.LockMessage;
                }
            });
        }

        public async Task<string?> GetUnlockMessageAsync(ulong guildId)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildMessageSettingsAsync(dbContext, guildId);
                    return settings.UnlockMessage;
                }
            });
        }

        private async Task<GuildSettings> GuildSettingsAsync(BotDbContext dbContext, ulong guildId)
        {
            var settings = await dbContext.GuildSettings.FindAsync(guildId);

            if (settings is null)
            {
                settings = new GuildSettings { GuildId = guildId };
                dbContext.GuildSettings.Add(settings);
            }

            return settings;
        }

        private async Task<GuildMessageSettings> GuildMessageSettingsAsync(BotDbContext dbContext, ulong guildId)
        {
            var settings = await dbContext.GuildMessageSettings.FindAsync(guildId);

            if (settings is null)
            {
                settings = new GuildMessageSettings { GuildId = guildId };
                dbContext.GuildMessageSettings.Add(settings);
            }

            return settings;
        }
    }
}