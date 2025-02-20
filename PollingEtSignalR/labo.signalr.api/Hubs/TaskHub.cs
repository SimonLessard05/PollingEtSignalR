using System;
using System.Threading.Tasks;
using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.SignalR;

namespace labo.signalr.api.Hubs
{
    public class TaskHub : Hub
    {
        ApplicationDbContext _context;

        public TaskHub(ApplicationDbContext context)
        {
            context = _context;
        }
        public async Task EnnvoyerListeConnection(int value, string connectionId)
        {
            await Clients.Client(connectionId).SendAsync();
        }
    }
}
