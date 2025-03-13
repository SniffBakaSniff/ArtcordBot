using System.ComponentModel;
using ArtcordBot.Helpers;
using ArtcordBot.Services;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace ArtcordBot.Features.ModerationCommands
{
    public partial class ModerationCommandGroup
    {
        [Command("lock")]
        [Description("Locks a channel or the whole server.")]
        public async Task LockAsync(CommandContext ctx, DiscordChannel? channel = null)
        {
            var targetChannel = channel ?? ctx.Channel;
            var everyoneRole = ctx.Guild!.EveryoneRole;
            var message = await _guildSettingsService.GetLockMessageAsync(ctx.Guild!.Id);
            var context = new EventContext(ctx);

            message ??= $"🔒 {targetChannel.Mention} has been locked.";
            var embed = MessageHelpers.GenericEmbed("Channel Has Been Locked!", message!, "ff0000");


            if (channel is not null && channel!.Type == DiscordChannelType.Category)
            {
                foreach (var child in channel.Children)
                {
                    await child.AddOverwriteAsync(everyoneRole, deny: DiscordPermissions.SendMessages);

                    if (message is not null)
                    {
                        message = _stringInterpolatorService.Interpolate(message, context);

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
                    message = _stringInterpolatorService.Interpolate(message, context);
                    await ctx.RespondAsync(embed);
                }
            }
        }
    }
}