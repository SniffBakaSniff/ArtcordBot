namespace ArtcordBot.Services.Database
{
    public enum MessageType
    {
        Appeal,
        Welcome,
        Farewell,
        MutedNotification,
        TicketCreation,
        TicketClosure,
        Error
    }

    public class MessageSettingsService : IMessageSettingsService
    {
        public async Task<string?> ManageMessageSettingAsync(ulong guildId, MessageType messageType, string? newMessage = null)
        {
            return await ExceptionHandler.HandleAsync(async () =>
            {
                using (var dbContext = new BotDbContext())
                {
                    var settings = await GuildMessageSettingsAsync(dbContext, guildId);

                    switch (messageType)
                    {
                        case MessageType.Appeal:
                            if (newMessage is not null)
                            {
                                settings.AppealMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.AppealMessage;

                        case MessageType.Welcome:
                            if (newMessage is not null)
                            {
                                settings.WelcomeMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.WelcomeMessage;

                        case MessageType.Farewell:
                            if (newMessage is not null)
                            {
                                settings.FarewellMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.FarewellMessage;

                        case MessageType.MutedNotification:
                            if (newMessage is not null)
                            {
                                settings.MutedNotificationMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.MutedNotificationMessage;

                        case MessageType.TicketCreation:
                            if (newMessage is not null)
                            {
                                settings.TicketCreationMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.TicketCreationMessage;

                        case MessageType.TicketClosure:
                            if (newMessage is not null)
                            {
                                settings.TicketClosureMessage = newMessage;
                                await dbContext.SaveChangesAsync();
                            }
                            return settings.TicketClosureMessage;

                        case MessageType.Error:
                            if (newMessage is not null)
                            {
                                settings.ErrorMessage = newMessage;
                            }
                            return settings.ErrorMessage;

                        default:
                            throw new ArgumentException("Invalid message type specified.");
                    }
                }
            });
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
