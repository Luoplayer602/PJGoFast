using Microsoft.AspNetCore.SignalR;

namespace PJGoFast.Hubs
{
    public class NotificationHub : Hub
    {
        // Gửi thông báo khi chuyến đi thay đổi
        public async Task NotifyTripChanged(string tripId, string driverId = null, string customerId = null)
        {
            if (!string.IsNullOrEmpty(driverId))
                await Clients.User(driverId).SendAsync("TripChanged", tripId);

            if (!string.IsNullOrEmpty(customerId))
                await Clients.User(customerId).SendAsync("TripChanged", tripId);

            await Clients.Group("Dispatchers").SendAsync("TripChanged", tripId);
        }

        // Gửi thông báo phân công
        public async Task NotifyAssignment(string tripId, string driverId)
        {
            await Clients.User(driverId).SendAsync("NewAssignment", tripId);
        }

        // Gửi thông báo cập nhật dashboard
        public async Task NotifyDashboardUpdate(string driverId = null)
        {
            if (!string.IsNullOrEmpty(driverId))
                await Clients.User(driverId).SendAsync("DashboardChanged");
            else
                await Clients.All.SendAsync("DashboardChanged");
        }

        // Gửi cập nhật timer phân công
        public async Task NotifyAssignmentTimerUpdate(string tripId, int secondsRemaining)
        {
            await Clients.Group("Dispatchers").SendAsync("AssignmentTimerUpdate", tripId, secondsRemaining);
        }

        // Thêm người dùng vào group
        public async Task JoinDispatcherGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Dispatchers");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                      ?? Context.User?.FindFirst("sub")?.Value
                      ?? Context.User?.FindFirst("nameidentifier")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User-{userId}");
            }

            await base.OnConnectedAsync();
        }
    }
}

