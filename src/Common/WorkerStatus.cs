using Apache.Ignite.Core.Binary;
using Microsoft.Extensions.Logging;

namespace Demo.Common;

public class WorkerStatus : IWorkerStatus, IBinarizable
{
    private readonly ILogger<WorkerStatus> _logger;

    public WorkerStatus(ILogger<WorkerStatus> logger, IDataGrid dataGrid)
    {
        _logger = logger;
        DataGrid = dataGrid ?? throw new ArgumentNullException(nameof(dataGrid));

        WorkerId = DataGrid.GetWorkerIdAsync().Result;
        _logger.LogInformation("WorkerStatus: found workerId: {WorkerId}", WorkerId);
        StartTime = DateTime.UtcNow;
        TopicList = new List<string>();

        AddTopicId("Unassigned");
    }

    public void AddTopicId(string topicId)
    {
        TopicId = topicId;
        if (!TopicList.Contains(topicId))
        {
            TopicList.Add(topicId);
        }
    }

    public long IncrementRequestsServed()
    {
        RequestsServed++;
        _logger.LogDebug("IncrementRequestsServed: requests served: {RequestsServed}", RequestsServed);

        return RequestsServed;
    }

    public void WriteBinary(IBinaryWriter writer)
    {
        writer.WriteLong("RequestsServed", RequestsServed);
        writer.WriteTimestamp("StartTime", StartTime);
        writer.WriteString("TopicId", TopicId);
        writer.WriteStringArray("TopicList", TopicList.ToArray());
        writer.WriteString("WorkerId", WorkerId);
    }

    public void ReadBinary(IBinaryReader reader)
    {
        RequestsServed = reader.ReadLong("RequestsServed");
        StartTime = reader.ReadTimestamp("StartTime");
        TopicId = reader.ReadString("TopicId");
        TopicList = reader.ReadStringArray("TopicList");
        WorkerId = reader.ReadString("WorkerId");
    }

    public long RequestsServed { get; set; }

    public DateTime? StartTime { get; set; }

    public string? TopicId { get; set; }

    public IList<string> TopicList { get; set; }

    public string? WorkerId { get; set; }

    private IDataGrid DataGrid { get; }
}
