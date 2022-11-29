#pragma once

#include <ctime>
#include <string>
#include <vector>
#include <ignite/ignition.h>

class WorkerStatus
{
	friend struct ignite::binary::BinaryType<WorkerStatus>;
public:
	WorkerStatus();
	WorkerStatus(const std::string& workerId);
	void addTopicId(const std::string& topicId);
	long incrementRequestsServed();

	std::string m_containerId;
	std::string m_workerId;
	time_t m_startTime;
	std::string m_tcVersion;
	std::string m_topicId;
	long m_topicReassignments = 0;
	long m_requestsServed = 0;
	std::vector<std::string> m_topicIds;
	long m_requestLimit = 0;
	long m_virtualMemoryInMB = 0;
	long m_virtualMemoryLimit = 0;
};

template<>
struct ignite::binary::BinaryType<WorkerStatus> : ignite::binary::BinaryTypeDefaultAll<WorkerStatus>
{
    static void GetTypeName(std::string& name)
    {
        name = "WorkerStatus";
    }

    static void Write(BinaryWriter& writer, const WorkerStatus& obj)
    {
        writer.WriteString("ContainerId", obj.m_containerId);
        writer.WriteInt64("RequestLimit", obj.m_requestLimit);
        writer.WriteInt64("RequestsServed", obj.m_requestsServed);
        writer.WriteTimestamp("StartTime", obj.m_startTime);
        writer.WriteString("TCVERSION", obj.m_tcVersion);
        writer.WriteString("TopicId", obj.m_topicId);

        auto topicListWriter = writer.WriteStringArray("TopicList");
        for (std::string t : obj.m_topicIds)
        {
            topicListWriter.Write(t);
        }
        topicListWriter.Close();

        writer.WriteInt64("TopicReassignments", obj.m_topicReassignments);
        writer.WriteInt64("VirtualMemoryInMB", obj.m_virtualMemoryInMB);
        writer.WriteInt64("VirtualMemoryLimit", obj.m_virtualMemoryLimit);
        writer.WriteString("WorkerId", obj.m_workerId);
    }

    static void Read(BinaryReader& reader, WorkerStatus& dst)
    {
        dst.m_containerId = reader.ReadString("ContainerId");
        dst.m_requestLimit = reader.ReadInt64("RequestLimit");
        dst.m_requestsServed = reader.ReadInt64("RequestsServed");
        dst.m_startTime = reader.ReadTimestamp("StartTime").GetSeconds();
        dst.m_tcVersion = reader.ReadString("TCVERSION");
        dst.m_topicId = reader.ReadString("TopicId");

        auto topicListReader = reader.ReadStringArray("TopicList");
        while (topicListReader.HasNext())
        {
            auto topicId = topicListReader.GetNext();
            dst.m_topicIds.push_back(topicId);
        }

        dst.m_topicReassignments = reader.ReadInt64("TopicReassignments");
        dst.m_virtualMemoryInMB = reader.ReadInt64("VirtualMemoryInMB");
        dst.m_virtualMemoryLimit = reader.ReadInt64("VirtualMemoryLimit");
        dst.m_workerId = reader.ReadString("WorkerId");
    }
};
