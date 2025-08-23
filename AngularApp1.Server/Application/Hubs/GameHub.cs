using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace AngularApp1.Server.Application.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinGame(long gameId) =>
        await Groups.AddToGroupAsync(Context.ConnectionId, $"game:{gameId}");
        public async Task LeaveGame(long gameId) =>
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"game:{gameId}");
    }
}
