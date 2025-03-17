using ArtcordBot.Helpers;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;


namespace ArtcordBot.Features.ConfigCommands
{
    public partial class ConfigCommandsGroup
    {
        [Command("addPreset")]
        public async Task AddPreset(CommandContext ctx, string name, string? channels = null, string? members = null)
        {
            var embed = MessageHelpers.GenericAddedPresetEmbed(
                $"Preset {name} Added!",
                $"Channels: {channels}\nMembers: {members}"
            );

            channels ??= "None";
            members ??= "None";

            var preset = await _guildPresetService.GetPresetAsync(ctx.Guild!.Id, name);

            if (preset is not null)
            {
                embed = MessageHelpers.GenericErrorEmbed(
                    title:"Preset Name Taken!",
                    message:$"The preset name ``{name}`` is already in use. Please choose a different name."
                );
                
                await ctx.RespondAsync(embed);
            }
            else
            {
                await _guildPresetService.AddPresetAsync(ctx.Guild!.Id, name, channels, members);
                await ctx.RespondAsync(embed);
            }
        }

        [Command("editPreset")]
        public async Task EditPreset(CommandContext ctx, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string name, 
                            string? newName = null, string? channels = null, string? members = null)
        {
            var guildId = ctx.Guild!.Id;

            await _guildPresetService.EditPresetAsync(guildId, name, newName, channels, members);

            newName ??= "Unchanged";
            channels ??= "Unchanged";
            members ??= "Unchanged";

            var embed = MessageHelpers.GenericUpdateEmbed(
                $"Preset {name} Updated!",
                $"Name: {newName}\nChannels: {channels}\nMembers: {members}",
                "00ff00"
            );

            await ctx.RespondAsync(embed);
        }


        [Command("removePreset")]
        public async Task RemovePreset(CommandContext ctx, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string name)
        {
            await _guildPresetService.RemovePresetChannelsAsync(ctx.Guild!.Id, name);
            var embed = MessageHelpers.GenericSuccessEmbed($"Preset {name} Deleted.", $"The preset ``{name}`` has successfully been deleted.");
            await ctx.RespondAsync(embed);
        }

        [Command("viewPreset")]
        public async Task ViewPreset(CommandContext ctx, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string name)
        {
            var preset = await _guildPresetService.GetPresetAsync(ctx.Guild!.Id, name);

            if (preset is null)
            {
                await ctx.RespondAsync("Preset not found.");
                return;
            }

            var embed = MessageHelpers.GenericViewPresetEmbed($"Preset: {name}", $"Id: {preset.PresetId}\nName: {name}\nChannels: {preset.Channels}\nMembers: {preset.Members}");
            await ctx.RespondAsync(embed);
        }

        [Command("listPresets")]
        public async Task ListPresets(CommandContext ctx)
        {
            var paginatedPresets = await _paginationService.GetPaginatedResults(dbContext.GuildPresets.Where(p => p.GuildId == ctx.Guild!.Id), 1, 5);

            var embed = new DiscordEmbedBuilder
            {
                Title = "Presets",
                Color = DiscordColor.Aquamarine,
                Timestamp = DateTime.UtcNow
            };

            foreach (var preset in paginatedPresets.Records)
            {
                embed.AddField($"{preset.Name}", $"Channels: {preset.Channels}\nMembers: {preset.Members}");
            }

            embed.WithFooter($"Page {paginatedPresets.CurrentPage}/{paginatedPresets.TotalPages}");

                var messageBuilder = new DiscordMessageBuilder()
                    .AddEmbed(embed.Build());

                if (paginatedPresets.TotalPages > 1 || paginatedPresets.CurrentPage > 1)
                {
                    messageBuilder.AddComponents(
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "previous_presetlist_page", "Previous", true),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "next_presetlist_page", "Next", false)
                    );
                }

                await ctx.RespondAsync(messageBuilder);
        }
    }


}