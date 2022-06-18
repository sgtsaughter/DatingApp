using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = 
            new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            // lock our dictionary so it an only do one thing at a time
            // Another user cannot access this dictionary while another user is accessing it.
            // This presents a problem with scalability that signalR or storing this in the database would fix.
            // With this strategry, we would only be able to have the application stored on a single server for this functionality to work
            // Right now this information is only stored in memory as a singleton (see ApplicationServiceExtensions.cs)
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {   
                    // For this dictionary username will be the key and the connectedId will be the value
                    OnlineUsers[username].Add(connectionId);
                } else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId });
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock(OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}
