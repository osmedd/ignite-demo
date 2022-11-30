using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Demo.Common;

namespace Demo.Worker.NET;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, IDataGrid dataGrid, IWorkerStatus workerStatus)
    {
        _logger = logger;
        DataGrid = dataGrid ?? throw new ArgumentNullException(nameof(dataGrid));
        WorkerStatus = workerStatus ?? throw new ArgumentNullException(nameof(workerStatus));
    }

    private IDataGrid DataGrid { get; }
    private IWorkerStatus WorkerStatus { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            if (string.IsNullOrWhiteSpace(WorkerStatus.WorkerId))
            {
                _logger.LogError("WorkerId is not set!  Skipping...");
                continue;
            }

            WorkerStatus.IncrementRequestsServed();
            await DataGrid.UpdateWorkerStatusAsync(WorkerStatus.WorkerId, (WorkerStatus)WorkerStatus);

            await Task.Delay(60000, stoppingToken);
        }
    }
}
