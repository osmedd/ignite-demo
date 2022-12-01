using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Expiry;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Microsoft.Extensions.Logging;

namespace Demo.Common;

public class DataGrid : IDataGrid, IDisposable
{
    private readonly ILogger<DataGrid> _logger;

    public DataGrid(ILogger<DataGrid> logger)
    {
        _logger = logger;

        ServerId = Guid.NewGuid();

        var cfg = new IgniteConfiguration
        {
            JavaPeerClassLoadingEnabled = true,
            MetricsLogFrequency = TimeSpan.Zero,
            ClientMode = true,
            DiscoverySpi = new TcpDiscoverySpi
            {
                IpFinder = new TcpDiscoveryStaticIpFinder
                {
                    Endpoints = new[] { "127.0.0.1" }
                }
            },
            BinaryConfiguration = new BinaryConfiguration(typeof(WorkerStatus))
            {
                CompactFooter = false,
                NameMapper = new BinaryBasicNameMapper
                {
                    IsSimpleName= true
                }
            }
        };
        //var configFile = Environment.GetEnvironmentVariable("DEFAULT_CONFIG");
        //if (!string.IsNullOrWhiteSpace(configFile))
        //{
        //    cfg.SpringConfigUrl = configFile;
        //}
        Client = Ignition.Start(cfg);
        if (!Client.GetCluster().IsActive())
        {
            Client.GetCluster().SetActive(true);
        }
        Initialized = true;
    }

    public bool TestComputeNode(string workerId)
    {
        if (!Initialized || Client is null)
        {
            throw new DataGridException("NewWorkerIdAsync: ignite client not available!");
        }

        bool found;

        try
        {
            var nodeId = new Guid(workerId);
            var nodeList = new List<Guid>() { nodeId };
            var compute = Client.GetCluster().ForNodeIds(nodeList).GetCompute();
            var call = new AssignTopicFunc(workerId);
            var result = compute.Call(call);
            _logger.LogInformation("TestComputeNode: got result: {Result}", result);
            found = true;
        }
        catch (Exception)
        {
            throw;
        }

        return found;
    }

    public Task<string> GetWorkerIdAsync()
    {
        if (!Initialized || Client is null)
        {
            throw new DataGridException("NewWorkerIdAsync: ignite client not available!");
        }

        var workerId = Client.GetCluster().GetLocalNode().ConsistentId.ToString();

        return Task.FromResult(workerId ?? "Unknown");
    }

    public Task UpdateWorkerStatusAsync(string workerId, WorkerStatus workerStatus)
    {
        var workerStatusCache = GetWorkerStatusCache();
        workerStatusCache.Put(workerId, workerStatus);

        return Task.CompletedTask;
    }

    public Task RemoveWorkerStatusAsync(string workerId)
    {
        throw new NotImplementedException();
    }

    // TODO Make ignite client private
    public IIgnite? Client { get; set; }
    private bool Initialized { get; set; }

    public Guid ServerId { get; }

    public Task<IEnumerable<WorkerStatus>> GetWorkerStatusAllAsync()
    {
        var workerStatusCache = GetWorkerStatusCache();
        var workers = workerStatusCache.Select(s => s.Value);
        return Task.FromResult(workers);
    }

    public async Task<WorkerStatus> GetWorkerStatusAsync(string workerId)
    {
        var workerStatusCache = GetWorkerStatusCache();

        WorkerStatus workerStatus;
        try
        {
            if (workerStatusCache.ContainsKey(workerId))
            {
                workerStatus = await workerStatusCache.GetAsync(workerId);
            }
            else
            {
                throw new DataGridException($"WorkerId {workerId} not found");
            }

        }
        catch (Exception ex)
        {
            throw new DataGridException("GetWorkerStatusAsync: unexpected exception", ex);
        }

        return workerStatus;
    }

    private ICache<string, WorkerStatus> GetWorkerStatusCache()
    {
        if (!Initialized || Client is null)
        {
            throw new DataGridException("GetWorkerStatusCache: ignite client not available!");
        }

        var expiryDuration = TimeSpan.FromSeconds(90);
        var workerStatusCache = Client.GetOrCreateCache<string, WorkerStatus>("WorkerStatus")
            .WithExpiryPolicy(new ExpiryPolicy(expiryDuration, expiryDuration, expiryDuration));

        return workerStatusCache;
    }

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                Client?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DataGrid()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
