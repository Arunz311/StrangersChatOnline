using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
namespace StrangersChat2.Hubs
{
    public class ChatHub : Hub
    {
        // Keep track of connected clients
        private static readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        // Handle client connections
        public override async Task OnConnectedAsync()
        {
            // Add connection ID to the dictionary
            _connections.TryAdd(Context.ConnectionId, null);
            await base.OnConnectedAsync();
        }

        // Handle client disconnections
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove connection ID from the dictionary
            _connections.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        // Start chat
        public async Task StartChat()
        {
            var availableConnections = _connections.Keys.Except(new[] { Context.ConnectionId }).ToList();

            if (availableConnections.Any())
            {
                var partnerConnectionId = availableConnections.First(); // Simple logic for now: pick the first available client
                _connections[Context.ConnectionId] = partnerConnectionId;
                _connections[partnerConnectionId] = Context.ConnectionId;

                // Notify both clients
                await Clients.Client(Context.ConnectionId).SendAsync("ConnectedToPartner", partnerConnectionId);
                await Clients.Client(partnerConnectionId).SendAsync("ConnectedToPartner", Context.ConnectionId);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("NoAvailablePartners");
            }
        }

        // Send a message to the connected partner
        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var partnerConnectionId))
            {
                await Clients.Client(partnerConnectionId).SendAsync("ReceiveMessage", message);
            }
        }

        // End chat
        public async Task EndChat()
        {
            if (_connections.TryRemove(Context.ConnectionId, out var partnerConnectionId))
            {
                if (partnerConnectionId != null)
                {
                    _connections.TryRemove(partnerConnectionId, out _);
                    await Clients.Client(partnerConnectionId).SendAsync("PartnerDisconnected");
                }
            }
        }
    }
}
