using Microsoft.AspNetCore.SignalR;
using squittal.ScrimPlanetmans.Hubs.Models;
using System.Threading.Tasks;

using System.Diagnostics;

namespace squittal.ScrimPlanetmans.Hubs
{
    public class MatchSetupHub : Hub
    {
        //public Task SendMessage(string message)
        //{
        //    Debug.WriteLine($"MatchSetupHub: SendMessage");

        //    return Clients.All.SendAsync("ReceiveMessage", message);
        //}

        //public Task SendTeamPlayerChangeMessage(TeamPlayerChangeMessage message)
        //{
        //    Debug.WriteLine($"MatchSetupHub: SendTeamPlayerChangeMessage");
            
        //    return Clients.All.SendAsync("ReceiveTeamPlayerChangeMessage", message);
        //}

        //public Task SendMessage(TeamPlayerChangeMessage message)
        //{
        //    Debug.WriteLine($"MatchSetupHub: SendMessage2");

        //    return Clients.All.SendAsync("ReceiveMessage", message);
        //}

        public Task SendMessage(string message)
        {
            Debug.WriteLine($"MatchSetupHub: SendMessage2");

            return Clients.All.SendAsync("ReceiveMessage", message);
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
