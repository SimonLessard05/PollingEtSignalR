using System;
using System.Threading.Tasks;
using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace labo.signalr.api.Hubs
{
    public class TaskHub : Hub
    {
        ApplicationDbContext _context;

        public static class UserHandler
        {
            public static HashSet<string> ConnectedIds = new HashSet<string>();
        }
        public TaskHub(ApplicationDbContext context)
        {
            context = _context;
        }
        
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // TODO: Ajouter votre logique
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
            List<UselessTask> tasks =  _context.UselessTasks.ToList();

            await Clients.Caller.SendAsync("TaskList", tasks);
        }
        public async Task AddTask(string taskText)
        {
            _context.UselessTasks.Add(new UselessTask() { Text = taskText });
            _context.SaveChanges();
            List<UselessTask> tasks = _context.UselessTasks.ToList();
            await Clients.All.SendAsync("TaskList", tasks);

        }

        public async Task CompleteTask(int id)
        {
            var task = _context.UselessTasks.Single(t => t.Id == id);
            task.Completed = true;
            _context.SaveChanges();
            List<UselessTask> tasks = _context.UselessTasks.ToList();
            await Clients.All.SendAsync("TaskList", tasks);

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            //Decrementer nombre d'utilisateurs actifs
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
            await Clients.All.SendAsync("UserCount");
        }
    }
}
