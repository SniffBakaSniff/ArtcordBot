using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

public class PresetNameAutoCompleteProvider : IAutoCompleteProvider
{
    public async ValueTask<IReadOnlyDictionary<string, object>> AutoCompleteAsync(AutoCompleteContext context)
    {
        if (context.ServiceProvider.GetService(typeof(IGuildPresetService)) is not IGuildPresetService presetService)
            return new Dictionary<string, object>();

        var guildId = context.Guild!.Id;
        var presetNamesCsv = await presetService.GetPresetNamesAsync(guildId);

        presetNamesCsv ??= "";

        IEnumerable<string> presetNames = presetNamesCsv!.Split(',').Select(name => name.Trim()).Where(name => !string.IsNullOrWhiteSpace(name));

        string userInput = context.UserInput ?? string.Empty;
    
        var choices = presetNames
            .Where(name => name.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(name => name, name => (object)name);

        return choices;
    }
}