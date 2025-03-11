using DSharpPlus;
using DSharpPlus.EventArgs;

namespace ArtcordBot.Listeners
{
    public class TicketMessageLogger
    {

        private readonly ITicketService _ticketService;

        public TicketMessageLogger(ITicketService ticketService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        }
       
        public async Task LogTicketMessages(DiscordClient client, MessageCreatedEventArgs e)
        {
            var (ticketId, userId) = await _ticketService.GetTicketIdForChannelAsync(e.Channel.Id);
            var messageType = e.Author.Id == userId ? TicketMessageType.Moderator : TicketMessageType.Submitter;
            if(ticketId.HasValue) 
            {
                await _ticketService.LogTicketMessageAsync(ticketId: ticketId.Value, userId: e.Author.Id, messageType: messageType, content: e.Message.Content);
            }
        }
    }
}
