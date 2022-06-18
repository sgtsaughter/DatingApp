using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            // when a client connects, we're going to update our precense tracker,
            // and we're going to send the updated list of current users back to everyone that's connected.
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if (!isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
            

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exceptoin)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId); 

            if(isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            
            await base.OnDisconnectedAsync(exceptoin);
        }
    }
}
