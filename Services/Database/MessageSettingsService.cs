namespace ArtcordBot.Services.Database
{
    public class MessageSettingsService : IMessageSettingsService
    {
        public async Task<string?> ManageMessageSettingAsync(ulong guildId, string messageType, string? newMessage = null)
        {
            try
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildMessageSettingsAsync(dbContext, guildId);

                    switch (messageType.ToLower())
                    {
                        case "appeal":
                            if (newMessage != null)
                            {
                                settings.AppealMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.AppealMessage;

                        case "welcome":
                            if (newMessage != null)
                            {
                                settings.WelcomeMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.WelcomeMessage;

                        case "farewell":
                            if (newMessage != null)
                            {
                                settings.FarewellMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.FarewellMessage;

                        case "mutednotification":
                            if (newMessage != null)
                            {
                                settings.MutedNotificationMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.MutedNotificationMessage;

                        case "ticketcreation":
                            if (newMessage != null)
                            {
                                settings.TicketCreationMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.TicketCreationMessage;

                        case "ticketclosure":
                            if (newMessage != null)
                            {
                                settings.TicketClosureMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.TicketClosureMessage;

                        case "error":
                            if (newMessage != null)
                            {
                                settings.ErrorMessage = newMessage;
                            }
                            return settings.ErrorMessage;

                        default:
                            throw new ArgumentException("Invalid message type specified.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in ManageMessageSettingAsync: {ex.Message}");
                return null;
            }
        }

        private async Task<GuildMessageSettings> GuildMessageSettingsAsync(BotDbContext dbContext, ulong guildId)
        {
            var settings = await dbContext.GuildMessageSettings.FindAsync(guildId);

            if (settings == null)
            {
                settings = new GuildMessageSettings { GuildId = guildId };
                dbContext.GuildMessageSettings.Add(settings);
            }

            return settings;
        }
    }
}
