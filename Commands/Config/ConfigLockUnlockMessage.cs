using System.ComponentModel;
using ArtcordBot.Helpers;
using ArtcordBot.Services.Database;
using DSharpPlus.Commands;

namespace ArtcordBot.Features.ConfigCommands
{
    public partial class ConfigCommandsGroup
    {
        [Command("lockmessage")]
        [Description("Configures the lock message")]
        public async Task ConfigLockMessageAsync(CommandContext ctx, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                await ctx.RespondAsync("Please provide a message for the lock message.");
                return;
            }

            await _messageSettingsService.ManageMessageSettingAsync(ctx.Guild!.Id, MessageType.LockMessage, message);
            
            await ctx.RespondAsync(
                MessageHelpers.GenericUpdateEmbed("Lock Message Updated!\n", extra: message)
            );
        }

        [Command("unlockmessage")]
        [Description("Configures the unlock message")]
        public async Task ConfigUnlockMessageAsync(CommandContext ctx, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                await ctx.RespondAsync("Please provide a message for the unlock message.");
                return;
            }

            await _messageSettingsService.ManageMessageSettingAsync(ctx.Guild!.Id, MessageType.UnlockMessage, message);

            await ctx.RespondAsync(
                MessageHelpers.GenericUpdateEmbed("Unlock Message Updated!\n", extra: message)
            );
        }
    }
}