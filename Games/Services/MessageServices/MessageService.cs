using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Games.Services.MessageServices
{
    public interface IMessageService
    {
        public Task SendMessage(string message);

    }
    public class MessageService : IMessageService
    {
        private readonly IHubContext<GamesHub> _hubContext;

        public MessageService(IHubContext<GamesHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessage(string message)
        {
            await _hubContext.Clients.All.SendAsync("Message", message);
        }
    }
}
