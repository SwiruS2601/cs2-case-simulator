using Microsoft.AspNetCore.SignalR;

namespace Cs2CaseOpener.Hubs;

public class UnboxHub : Hub
{
    public async Task BroadcastRareUnbox(string country, string itemName, string rarity, string imageUrl)
    {
        await Clients.All.SendAsync("receiveRareUnbox", country, itemName, rarity, imageUrl);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}