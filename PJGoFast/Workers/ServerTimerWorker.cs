using PJGoFast.Services.Interfaces;

namespace PJGoFast.Workers
{
    public class ServerTimerWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServerTimerWorker> _logger;

        public ServerTimerWorker(IServiceScopeFactory scopeFactory, ILogger<ServerTimerWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
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
