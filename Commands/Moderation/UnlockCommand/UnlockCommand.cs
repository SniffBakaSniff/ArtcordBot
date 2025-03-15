using System.ComponentModel;
using ArtcordBot.Helpers;
using ArtcordBot.Services;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup 
    {
        [Command("unlock")]
        [Description("Unlocks a channel or the whole server.")]
        public async Task UnlockAsync(CommandContext ctx, DiscordChannel? channel = null, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string? preset = null, bool guild = false)
        {
            var targetChannel = channel ?? ctx.Channel;
            var everyoneRole = ctx.Guild!.EveryoneRole;
            var message = await _guildSettingsService.GetUnlockMessageAsync(ctx.Guild!.Id);
            var context = new EventContext(ctx);

            message ??= $"🔓 {targetChannel.Mention} has been unlocked.";
            message = _stringInterpolatorService.Interpolate(message, context);
            var embed = MessageHelpers.GenericEmbed("Channel Has Been Unlocked!", message!, "#00ff00");

            if (channel is not null && channel.Type == DiscordChannelType.Category)
            {
                foreach (var child in channel.Children)
                {
                    await child.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);

                    if (message is not null)
                    {
                        if (child == ctx.Channel)
                        {
                            await ctx.RespondAsync(embed);
                        }
                        else
                        {
                            await child.SendMessageAsync(embed);
                        }
                    }
                }
            }

            if (preset is not null)
            {
                string? channels = await _guildPresetService.GetPresetChannelsAsync(ctx.Guild.Id, preset) ?? string.Empty;
                ulong[] channelIds = channels!.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToArray();
                
                foreach (ulong presetChannels in channelIds)
                {
                    channel = await ctx.Guild.GetChannelAsync(presetChannels);
                    
                    if (channel is not null && channel!.Type == DiscordChannelType.Category)
                    {
                        foreach (var child in channel.Children)
                        {
                            await channel.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);

                            if (message is not null)
                            {
                                if (child == ctx.Channel)
                                {
                                    await ctx.RespondAsync(embed);
                                }
                                else
                                {
                                    await child.SendMessageAsync(embed);
                                }
                            }
                        }
                        break;
                    }

                    await channel!.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);
                }

                if (!channelIds.Contains(ctx.Channel.Id))
                {
                    message = $"All channels for the {preset} preset have been unlocked.";
                    embed = MessageHelpers.GenericEmbed($"Channels have been unlocked!", message, "00ff00");
                    await ctx.RespondAsync(embed);
                }
            }

            if (guild is true)
            {
                var channels = await ctx.Guild.GetChannelsAsync();

                foreach (var child in channels)
                {
                    await child!.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);

                    if (child.Type is not DiscordChannelType.Category)
                    {
                        if (child == ctx.Channel)
                        {
                            await ctx.RespondAsync(embed);
                        }
                        else
                        {
                            await child.SendMessageAsync(embed);
                        }
                    }
                }
            }

            else
            {
                await targetChannel.AddOverwriteAsync(everyoneRole, allow: DiscordPermissions.SendMessages);

                if (message is not null)
                {
                    await ctx.RespondAsync(embed);
                }
            }
        }
    }
}
