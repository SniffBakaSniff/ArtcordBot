using DSharpPlus.Commands;
using DSharpPlus.Entities;
using ArtcordBot.Helpers;

namespace ArtcordBot.Features.ModerationCommands
{

    public partial class ModerationCommandGroup {
    [Command("purge")]
    public class PurgeCommand
    {
        [Command("amount")]
        [System.ComponentModel.Description("Purges a specified number of messages reached from the channel.")]
        public static async Task PurgeAmountAsync(CommandContext context, 
            [System.ComponentModel.Description("The amount of messages to purge from the channel. (1 to 250, default 10)")] int amount = 10)
        {
            try
            {
                // Ensure the count is within range
                if (amount < 1 || amount > 250)
                {
                    await context.RespondAsync(
                        MessageHelpers.GenericErrorEmbed("A number from **1** to **250** must be provided for the parameter `amount`.")
                    );
                    return;
                }

                // Collect messages into a list
                var messages = new List<DiscordMessage>();

                await foreach (var message in context.Channel.GetMessagesAsync(amount, CancellationToken.None))
                {
                    messages.Add(message);
                }

                await RemoveMessagesAndRespondAsync(context, messages, context.Channel);
            }
            catch (ConversionHelpers.Exception ex)
            {
                await context.RespondAsync(
                    ex.ToEmbed()
                );
            }
            catch (Exception ex)
            {
                await context.RespondAsync(
                    MessageHelpers.GenericErrorEmbed($"An error occurred while purging messages:\n> {ex.Message}")
                );
            }
        }

        [Command("until")]
        [System.ComponentModel.Description("Purges until a certain message is reached from the channel.")]
        public static async Task PurgeUntilAsync(CommandContext context,
            [System.ComponentModel.Description("The message link or ID to delete until.")] string message,
            [System.ComponentModel.Description("Whether or not to delete the provided message as well. (default False)")] bool inclusive = false)
        {
            try
            {
                // Collect messages into a list
                var messages = new List<DiscordMessage>();

                DiscordMessage discordMessage = await ConversionHelpers.GetMessage(message, nameof(message), context);

                await foreach (var currentMessage in discordMessage.Channel!.GetMessagesAfterAsync(discordMessage.Id))
                {
                    messages.Add(currentMessage);
                }
                if (inclusive)
                {
                    messages.Add(discordMessage);
                }

                await RemoveMessagesAndRespondAsync(context, messages, discordMessage.Channel);
            }
            catch (ConversionHelpers.Exception ex)
            {
                await context.RespondAsync(
                    ex.ToEmbed()
                );
            }
            catch (Exception ex)
            {
                await context.RespondAsync(
                    MessageHelpers.GenericErrorEmbed($"An error occurred while purging messages:\n> {ex.Message}")
                );
            }
        }

        private static async Task RemoveMessagesAndRespondAsync(CommandContext context, List<DiscordMessage> messages, DiscordChannel channel)
        {
            // Bulk delete messages
            if (messages.Count > 0)
            {
                await channel.DeleteMessagesAsync(messages);
            }
            else
            {
                await context.RespondAsync(
                    MessageHelpers.GenericErrorEmbed("No messages to delete.")
                    );
                return;
            }

            await context.RespondAsync(
                    MessageHelpers.GenericSuccessEmbed("Purge complete!", $"Successfully deleted {messages.Count} message{(messages.Count == 1 ? "" : "s")}" +
                    $"{(channel.Id != context.Channel.Id ? $" in {channel.Mention}" : "")}.")
                );
        }
    }
}
}
