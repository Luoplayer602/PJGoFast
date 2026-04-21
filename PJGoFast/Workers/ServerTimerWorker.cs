using Microsoft.AspNetCore.SignalR;
using PJGoFast.Hubs;
using PJGoFast.Services.Interfaces;

namespace PJGoFast.Workers
{
    public class ServerTimerWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServerTimerWorker> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ServerTimerWorker(IServiceScopeFactory scopeFactory, ILogger<ServerTimerWorker> logger, IHubContext<NotificationHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var chuyenDiService = scope.ServiceProvider.GetRequiredService<IChuyenDiService>();
                    chuyenDiService.XuLyPhanCongHetHanTuDong();

                    // Gửi cập nhật timer cho các tài xế có chuyến được phân công
                    await _hubContext.Clients.All.SendAsync("TimerUpdate");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong ServerTimerWorker.");
                }
            }
        }
    }
}
