using Microsoft.AspNetCore.SignalR;

namespace Games.Services.MessageServices
{
    public class GamesHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("Message", message);
        }
    }
}
