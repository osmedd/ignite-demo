namespace Cfg.Common;

public interface IWorkerStatus
{
    long RequestsServed { get; set; }
    DateTime? StartTime { get; set; }
    string? TopicId { get; set; }
    IList<string>? TopicList { get; }
    string? WorkerId { get; set; }

    void AddTopicId(string topicId);
    long IncrementRequestsServed();
}
