using Cfg.Common;
using Microsoft.AspNetCore.Mvc;

namespace Cfg.RequestProcessor.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfiguratorController : ControllerBase
{
    private readonly ILogger<ConfiguratorController> _logger;

    public ConfiguratorController(ILogger<ConfiguratorController> logger, IDataGrid dataGrid)
    {
        _logger = logger;
        DataGrid = dataGrid;
    }

    [Route("test/compute/{workerId}")]
    [HttpGet]
    public bool TestComputeNode(string workerId)
    {
        var result = DataGrid.TestComputeNode(workerId);
        return result;
    }

    [Route("workers")]
    [HttpGet]
    public async Task<IEnumerable<WorkerStatus>> GetWorkers()
    {
        var workers = await DataGrid.GetWorkerStatusAllAsync();
        return workers;
    }

    [Route("workers/{workerId}")]
    [HttpGet]
    public async Task<ActionResult<WorkerStatus>> GetWorkerStatus(string workerId)
    {
        WorkerStatus workerStatus;

        if (string.IsNullOrWhiteSpace(workerId))
        {
            throw new ArgumentException($"'{nameof(workerId)}' cannot be null or whitespace.", nameof(workerId));
        }

        try
        {
            workerStatus = await DataGrid.GetWorkerStatusAsync(workerId);
        }
        catch (DataGridException ex)
        {
            _logger.LogError("Unexpected error.", ex);
            return StatusCode(StatusCodes.Status404NotFound, "WorkerId not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError("Unexpected general error.", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error.");
        }

        return Ok(workerStatus);
    }

    private IDataGrid DataGrid { get; }
}
