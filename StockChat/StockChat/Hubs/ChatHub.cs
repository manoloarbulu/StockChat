using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StockChat.Hubs
{
    [Authorize]
    public class ChatHub: Hub
    {
        public async Task Send(string username, string message)
        {
            //if (string.IsNullOrWhiteSpace(message))
            //    return;

            await Clients.All.SendAsync("Send", username, message);
        }

        public async void BotMessage(string username, string message)
        {
            //Will receive the message with command and the Stock ID
            //This must invoke the Bot that will consume the stock endpoint

            //await Task.CompletedTask;
            await Clients.All.SendAsync("BotMessage", username, message);
        }
    }
}
