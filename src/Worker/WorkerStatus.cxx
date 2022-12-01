#include "WorkerStatus.hxx"

WorkerStatus::WorkerStatus() : m_workerId("UNSET")
{
    // empty
}

WorkerStatus::WorkerStatus(const std::string& workerId) : m_workerId(workerId)
{
    m_startTime = std::time(0);
    addTopicId("Unassigned");
}

void WorkerStatus::addTopicId(const std::string& topicId)
{
    m_topicId = topicId;
    if (find(m_topicIds.begin(), m_topicIds.end(), topicId) == m_topicIds.end())
    {
        m_topicIds.push_back(topicId);
    }
}

long WorkerStatus::incrementRequestsServed()
{
    m_requestsServed++;
    return m_requestsServed;
}
