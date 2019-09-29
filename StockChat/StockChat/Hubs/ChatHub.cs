using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StockChat.Hubs
{
    [Authorize]
    public class ChatHub: Hub
    {
        public async Task Send(string username, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            await Clients.All.SendAsync("Send", username, message);
        }

        public async Task<HttpResponseMessage> BotMessage(string username, string message)
        {
            //Will receive the message with command and the Stock ID
            var stockId = Regex.Replace(message, @"\s", "").Replace("/stock=","", StringComparison.InvariantCultureIgnoreCase);
            if (string.IsNullOrWhiteSpace(stockId))
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            //Consume Bot endpoint
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("User-Agent","Stock Bot");
                return await client.GetAsync("https://localhost:44394/api/stock/"+stockId);
            }
        }

        internal async Task SendBotMessage(string message)
        {
            if (Clients != null)
                await Clients.All.SendAsync("BotMessage", "Stock Bot", message);
        }
    }
}
