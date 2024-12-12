using DataFileCreator;
using DataFileGrpcService.Common;

namespace DataFileGrpcService.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Hosted Service is working.");
            if (!DataFileService.FileExists(Constants.DataFileName))
            {
                var created = DataFileService.CreateFile(Constants.TempFileName, Constants.CopyCount);
                if (created) DataFileService.RenameFile(Constants.TempFileName, Constants.DataFileName);
                DataFileService.DeleteFile(Constants.TempFileName);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}