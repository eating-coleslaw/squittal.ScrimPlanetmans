using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Hubs
{
    public class EventHub : Hub
    {
        public Task SendMessage(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
