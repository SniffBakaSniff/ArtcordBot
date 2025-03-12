using System.Collections.Generic;
using DSharpPlus.Commands;

namespace ArtcordBot.Services
{
    public class StringInterpolatorService : IStringInterpolatorService
    {
        public string Interpolate(string template, CommandContext ctx)
        {
            var placeholders = new Dictionary<string, string>
            {
                { "{{username}}", ctx.User.Username },
                { "{{userid}}", ctx.User.Id.ToString() },
                { "{{channel}}", ctx.Channel.Name },
                { "{{server}}", ctx.Guild?.Name ?? "DM" },
                { "{{mention}}", ctx.User.Mention },
                { "{{nickname}}", ctx.Member?.Nickname ?? ctx.User.Username },
                { "{{useravatar}}", ctx.User.AvatarUrl },
                { "{{guildid}}", ctx.Guild?.Id.ToString() ?? "DM" },
                { "{{channelid}}", ctx.Channel.Id.ToString() },
                { "{{timestamp}}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
                { "{{command}}", ctx.Command.Name },
                { "{{args}}", string.Join(", ", ctx.Arguments) }
            };

            foreach (var placeholder in placeholders)
            {
                template = template.Replace(placeholder.Key, placeholder.Value);
            }

            return template;
        }
    }

    public interface IStringInterpolatorService
    {
        string Interpolate(string template, CommandContext ctx);
    }
}