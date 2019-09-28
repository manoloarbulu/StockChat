using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StockChat
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async void BotMessage(string user, string message)
        {
            //Will receive the message with command and the Stock ID
            //This must invoke the Bot that will consume the stock endpoint

            await Task.CompletedTask;
        }
    }
}
