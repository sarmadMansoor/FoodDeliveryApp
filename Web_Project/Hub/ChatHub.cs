
using Microsoft.AspNetCore.SignalR;

namespace Web_Project.Hubs
{
    public class ChatHub:Hub
    {
        public async Task NotifyCartUpdate(string productName, string username)
        {
            await Clients.Others.SendAsync("ReceiveCartUpdate", productName,username);
        }
    }
}
