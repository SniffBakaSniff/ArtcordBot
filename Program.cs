using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using ArtcordBot.Features;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using ArtcordBot.Features.ConfigCommands;
using ArtcordBot.Features.ModerationCommands;
using ArtcordBot.Listeners;
using ArtcordBot.Services.Database;

namespace ArtcordBot
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string? discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                Console.WriteLine("Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
                Environment.Exit(1);
            }

            DiscordClientBuilder builder = DiscordClientBuilder
                .CreateDefault(discordToken, TextCommandProcessor.RequiredIntents | SlashCommandProcessor.RequiredIntents | DiscordIntents.MessageContents | DiscordIntents.GuildMembers)
                .ConfigureServices(services => 
                {
                    services.AddDbContext<BotDbContext>();
                    services.AddScoped<IPrefixResolver, CustomPrefixResolver>();
                    services.AddScoped<IBanService, BanService>();
                    services.AddScoped<IGuildSettingsService, GuildSettingsService>();
                    services.AddScoped<IMessageSettingsService, MessageSettingsService>();
                    services.AddScoped<ITicketService, TicketService>();
                    services.AddScoped<IPaginationService, PaginationService>();
                });


            var buttonInteractionHandler = new ButtonInteractionListener(new TicketService(), new PaginationService());
            var ticketMessageLogger = new TicketMessageLogger(new TicketService());
            var joinLeaveListener = new JoinLeaveListener(new MessageSettingsService(), new GuildSettingsService());

            builder.ConfigureEventHandlers(b =>
            {
                b.HandleComponentInteractionCreated(buttonInteractionHandler.HandleButtonInteraction);
                b.HandleMessageCreated(ticketMessageLogger.LogTicketMessages);
                b.HandleGuildMemberAdded(joinLeaveListener.OnMemberJoined);
                b.HandleGuildMemberRemoved(joinLeaveListener.OnMemberLeft);
            });

            // Use the commands extension
            builder.UseCommands
            (
                // we register our commands here
                extension =>
                {
                    extension.AddCommands([
                        typeof(EchoCommand),
                        typeof(PingCommand),
                        typeof(ConfigCommandsGroup),
                        typeof(ModerationCommandGroup),
                        typeof(TicketCommandGroup)]);
                    TextCommandProcessor textCommandProcessor = new(new TextCommandConfiguration
                    {
                       // PrefixResolver = new DefaultPrefixResolver(true, "?", ".").ResolvePrefixAsync
                    });

                    // Add text commands with a custom prefix (?ping)
                    extension.AddProcessors(textCommandProcessor);

                    extension.CommandErrored += EventHandlers.CommandErrored;
                },

                
                new CommandsConfiguration()
                {
                    DebugGuildId = 1345544197310255134,
                    RegisterDefaultCommandProcessors = true,
                    UseDefaultCommandErrorHandler = false
                }
            );

            DiscordClient client = builder.Build();

            DiscordActivity status = new("Subaka", DiscordActivityType.ListeningTo);

            await client.ConnectAsync(status, DiscordUserStatus.Online);

            await Task.Delay(-1);
        }
    }
}
