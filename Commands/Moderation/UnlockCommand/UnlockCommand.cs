using System.ComponentModel;
using ArtcordBot.Helpers;
using ArtcordBot.Services;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup
    {
        [Command("unlock")]
        [Description("Unlocks a channel or a group of preset channels.")]
        public async Task UnlockAsync(CommandContext ctx, DiscordChannel? channel = null, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string? preset = null)
        {
            var targetChannel = channel ?? ctx.Channel;
            var everyoneRole = ctx.Guild!.EveryoneRole;
            var message = await _guildSettingsService.GetUnlockMessageAsync(ctx.Guild!.Id);
            var context = new EventContext(ctx);

            message ??= $"🔓 {targetChannel.Mention} has been unlocked.";
            message = _stringInterpolatorService.Interpolate(message, context);
            var embed = MessageHelpers.GenericEmbed("Channel Has Been Unlocked!", message!, "#00ff00");

            if (channel is not null && preset is not null)
            {
                await ctx.RespondAsync(MessageHelpers.GenericErrorEmbed(
                    title: "Multiple Parameters Selected!",
                    message: "Please select either **channel** or **preset**, you cannot select both."));
                return;
            }

            if (channel is not null)
            {
                await UnlockCategoryAsync(channel, everyoneRole, embed, ctx, sendResponse: true);
                return;
            }

            if (preset is not null)
            {
                await UnlockPresetChannelsAsync(ctx, preset, everyoneRole, embed);
                return;
            }

            await targetChannel.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);
            await ctx.RespondAsync(embed);
        }

        private async Task UnlockCategoryAsync(DiscordChannel channel, DiscordRole everyoneRole, DiscordEmbed embed, CommandContext ctx, bool sendResponse = true)
        {
            if (channel.Type == DiscordChannelType.Category)
            {
                foreach (var child in channel.Children)
                {
                    await UnlockSingleChannelAsync(child, everyoneRole, embed, ctx, sendResponse: false);
                }

                if (sendResponse)
                {
                    var categoryEmbed = MessageHelpers.GenericEmbed(
                        $"Category: {channel.Mention} has been unlocked!",
                        $"All channels within {channel.Mention} have been unlocked.",
                        "#00ff00");
                    await ctx.RespondAsync(categoryEmbed);
                }
            }
            else
            {
                await UnlockSingleChannelAsync(channel, everyoneRole, embed, ctx, sendResponse: sendResponse);
                if (sendResponse)
                {
                    await ctx.RespondAsync(embed);
                }
            }
        }

        private async Task UnlockSingleChannelAsync(DiscordChannel channel, DiscordRole everyoneRole, DiscordEmbed embed, CommandContext ctx, bool sendResponse = true)
        {
            if (channel.Type == DiscordChannelType.Text)
            {
                await channel.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);
                if (sendResponse && channel != ctx.Channel)
                {
                    await channel.SendMessageAsync(embed);
                }
            }
            else if (channel.Type == DiscordChannelType.Voice)
            {
                await channel.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.UseVoice);
            }
        }

        private async Task UnlockPresetChannelsAsync(CommandContext ctx, string preset, DiscordRole everyoneRole, DiscordEmbed embed)
        {
            ulong[] channelIds = await _guildPresetService.GetPresetChannelsAsync(ctx.Guild!.Id, preset) ?? [];
            foreach (ulong presetChannelId in channelIds)
            {
                var channel = await ctx.Guild.GetChannelAsync(presetChannelId);
                await UnlockCategoryAsync(channel, everyoneRole, embed, ctx, sendResponse: false);
            }

            var presetEmbed = MessageHelpers.GenericEmbed(
                $"Channels have been unlocked!",
                $"All channels for the **{preset}** preset have been unlocked.",
                "#00ff00");
            await ctx.RespondAsync(presetEmbed);
        }
    }
}