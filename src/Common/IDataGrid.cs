using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Common;

public interface IDataGrid
{
    Task<string> GetWorkerIdAsync();
    Task RemoveWorkerStatusAsync(string workerId);
    Task UpdateWorkerStatusAsync(string workerId, WorkerStatus workerStatus);
     Task<WorkerStatus> GetWorkerStatusAsync(string workerId);
    Task<IEnumerable<WorkerStatus>> GetWorkerStatusAllAsync();
    bool TestComputeNode(string workerId);
}
