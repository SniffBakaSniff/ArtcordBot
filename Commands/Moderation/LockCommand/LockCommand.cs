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
        [Description("Locks a channel or the whole server.")]
        public async Task LockAsync(CommandContext ctx, DiscordChannel? channel = null, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string? preset = null, bool guild = false)
        {
            var targetChannel = channel ?? ctx.Channel;
            var everyoneRole = ctx.Guild!.EveryoneRole;
            var message = await _guildSettingsService.GetLockMessageAsync(ctx.Guild!.Id);
            var context = new EventContext(ctx);

            message ??= $"🔒 {targetChannel.Mention} has been locked.";
            message = _stringInterpolatorService.Interpolate(message, context);
            var embed = MessageHelpers.GenericEmbed("Channel Has Been Locked!", message!, "ff0000");


            if (channel is not null && channel.Type == DiscordChannelType.Category)
            {
                foreach (var child in channel.Children)
                {
                    await child.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);

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
                            await child.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);

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

                    await channel!.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);

                    if (channel == ctx.Channel)
                    {
                        await ctx.RespondAsync(embed);
                    }
                    else
                    {
                        await channel.SendMessageAsync(embed);
                    }
                }

                if (!channelIds.Contains(ctx.Channel.Id))
                {
                    message = $"All channels for the {preset} preset have been locked.";
                    embed = MessageHelpers.GenericEmbed($"Channels have been locked!", message, "ff0000");
                    await ctx.RespondAsync(embed);
                }
            }

            if (guild is true)
            {
                var channels = await ctx.Guild.GetChannelsAsync();

                foreach (var child in channels)
                {
                    await child!.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);

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
                await targetChannel.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);

                if (message is not null)
                {
                    await ctx.RespondAsync(embed);
                }
            }
        }
    }
}