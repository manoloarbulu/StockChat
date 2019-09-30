using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StockChat.Models;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using StockChat.Configuration;

namespace StockChat.Hubs
{
    public class ChatHub: Hub
    {
        [Authorize]
        public async Task Send(string username, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            await Clients.All.SendAsync("Send", username, message);
        }

        [Authorize]
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
                return await client.GetAsync(ApiConfiguration.ApiUrl + stockId);
            }
        }

        [AllowAnonymous]
        public async Task SendBotMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var serializer = new DataContractJsonSerializer(typeof(StockModel));
            StockModel stock = null;

            using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                stock = (StockModel)serializer.ReadObject(ms);
            }

            //Sending back stock object to all the clients
            if (Clients != null && stock != null)
                await Clients.All.SendAsync("BotMessage", "Stock Bot", stock);
        }
    }
}
