using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;


namespace ArtcordBot.Features.ConfigCommands
{
    public partial class ConfigCommandsGroup
    {
        [Command("addPreset")]
        public async Task AddPreset(CommandContext ctx, string name, string channels)
        {
            string channelIds = channels.Replace("<", "").Replace("#", "").Replace(">", "");
            await _guildPresetService.SetPresetChannelsAsync(ctx.Guild!.Id, name, channelIds);
            await ctx.RespondAsync($"Adding Preset: {name} for channels:```{channelIds}```");
        }

        [Command("editPreset")]
        public async Task EditPreset(CommandContext ctx, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string name)
        {
            await ctx.RespondAsync($"This Dont Work Yet xD");
        }

        [Command("removePreset")]
        public async Task RemovePreset(CommandContext ctx, [SlashAutoCompleteProvider(typeof(PresetNameAutoCompleteProvider))] string name)
        {
            await _guildPresetService.RemovePresetChannelsAsync(ctx.Guild!.Id, name);
            await ctx.RespondAsync($"Removing Preset: {name}");
        }
    }


}