using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Hubs
{
    public class EventHub : Hub
    {
        public Task SendMessage(string message)
        {
            //Debug.WriteLine($"MatchSetupHub: SendMessage2");

            return Clients.All.SendAsync("ReceiveMessage2", message);
        }

        public Task SendMessage2(string message)
        {
            return Clients.All.SendAsync("ReceiveMessageAgain", message);
        }

        //public Task SendPlayerLoginMessage(string message)
        //{
        //    return Clients.All.SendAsync("ReceivePlayerLoginMessage", message);
        //}

        //public Task SendPlayerLogoutMessage(string message)
        //{
        //    return Clients.All.SendAsync("ReceivePlayerLogoutMessage", message);
        //}
    }
}
