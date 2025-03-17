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
        [Command("lock")]
        [Description("Locks a channel or a group of preset channels.")]
        public async Task LockAsync(CommandContext ctx, DiscordChannel? channel = null, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string? preset = null)
        {
            var targetChannel = channel ?? ctx.Channel;
            var everyoneRole = ctx.Guild!.EveryoneRole;
            var message = await _guildSettingsService.GetLockMessageAsync(ctx.Guild!.Id);
            var context = new EventContext(ctx);

            message ??= $"🔒 {targetChannel.Mention} has been locked.";
            message = _stringInterpolatorService.Interpolate(message, context);
            var embed = MessageHelpers.GenericEmbed("Channel Has Been Locked!", message!, "#ff0000");

            if (channel is not null && preset is not null)
            {
                await ctx.RespondAsync(MessageHelpers.GenericErrorEmbed(
                    title: "Multiple Parameters Selected!",
                    message: "Please select either **channel** or **preset**, you cannot select both."));
                return;
            }

            if (channel is not null)
            {
                await LockCategoryAsync(channel, everyoneRole, embed, ctx, sendResponse: true);
                return;
            }

            if (preset is not null)
            {
                await LockPresetChannelsAsync(ctx, preset, everyoneRole, embed);
                return;
            }

            await targetChannel.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);
            await ctx.RespondAsync(embed);
        }

        private async Task LockCategoryAsync(DiscordChannel channel, DiscordRole everyoneRole, DiscordEmbed embed, CommandContext ctx, bool sendResponse = true)
        {
            if (channel.Type == DiscordChannelType.Category)
            {
                foreach (var child in channel.Children)
                {
                    await LockSingleChannelAsync(child, everyoneRole, embed, ctx, sendResponse: false);
                }

                if (sendResponse)
                {
                    var categoryEmbed = MessageHelpers.GenericEmbed(
                        $"Category: {channel.Mention} has been locked!",
                        $"All channels within {channel.Mention} have been locked.",
                        "#ff0000");
                    await ctx.RespondAsync(categoryEmbed);
                }
            }
            else
            {
                await LockSingleChannelAsync(channel, everyoneRole, embed, ctx, sendResponse: sendResponse);
                if (sendResponse)
                    await ctx.RespondAsync(embed);
            }
        }

        private async Task LockSingleChannelAsync(DiscordChannel channel, DiscordRole everyoneRole, DiscordEmbed embed, CommandContext ctx, bool sendResponse = true)
        {
            if (channel.Type == DiscordChannelType.Text)
            {
                await channel.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);
                if (sendResponse && channel != ctx.Channel)
                {
                    await channel.SendMessageAsync(embed);
                }
            }
            else if (channel.Type == DiscordChannelType.Voice)
            {
                await channel.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.UseVoice);
            }
        }

        private async Task LockPresetChannelsAsync(CommandContext ctx, string preset, DiscordRole everyoneRole, DiscordEmbed embed)
        {
            ulong[] channelIds = await _guildPresetService.GetPresetChannelsAsync(ctx.Guild!.Id, preset) ?? [];
            foreach (ulong presetChannelId in channelIds)
            {
                var channel = await ctx.Guild.GetChannelAsync(presetChannelId);
                await LockCategoryAsync(channel, everyoneRole, embed, ctx, sendResponse: false);
            }

            var presetEmbed = MessageHelpers.GenericEmbed(
                $"Channels have been locked!",
                $"All channels for the **{preset}** preset have been locked.",
                "#ff0000");
            await ctx.RespondAsync(presetEmbed);
        }
    }
}
