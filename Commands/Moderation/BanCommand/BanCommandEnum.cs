using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

public enum MessageDeletionTimeframe
{
    [ChoiceDisplayName("None")]
    None = 0,

    [ChoiceDisplayName("Past Hour")]
    OneHour = 1,

    [ChoiceDisplayName("Past Day")]
    OneDay = 24,

    [ChoiceDisplayName("Past Week")]
    OneWeek = 168
}

public enum AppealStatus

{
    Pending,
    Approved,
    Denied
}