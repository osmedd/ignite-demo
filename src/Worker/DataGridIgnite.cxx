#include "DataGridIgnite.hxx"

#include <iostream>

namespace
{
    const char* CACHE_CONFIGURATION = "Configuration";
}

DataGridIgnite::DataGridIgnite()
{
    std::cerr << "DataGridIgnite:  constructing..." << std::endl;

    ignite::IgniteConfiguration cfg;
    auto configFile = getenv("DEFAULT_CONFIG");
    if (configFile != NULL)
    {
        std::cerr << "DataGridIgnite: found config file: " << configFile << std::endl;
        cfg.springCfgPath = configFile;
    }

    m_ignite = std::make_shared<ignite::Ignite>(ignite::Ignition::Start(cfg));
    if (!m_ignite->IsActive())
    {
        m_ignite->SetActive(true);
    }
}

DataGridIgnite::~DataGridIgnite()
{
    try
    {
        ignite::Ignition::StopAll(false);
    }
    catch (const std::exception&)
    {
        std::cerr << "DataGridIgnite:  caught error in deconstructor!" << std::endl;
    }
}

std::string DataGridIgnite::readCurrentLogLevel()
{
    return std::string();
}

std::string DataGridIgnite::getWorkerId()
{
    std::string workerId = m_ignite->GetCluster().GetLocalNode().GetConsistentId();
    std::cerr << "getWorkerId: found name: '" << workerId << "'" << std::endl;
    return workerId;
}

void DataGridIgnite::updateWorkerStatus(const std::string& workerId, WorkerStatus& workerStatus)
{
    std::cerr << "updateWorkerStatus: updating worker " << workerId << std::endl;

    ignite::cache::Cache<std::string, WorkerStatus> workerStatusCache = m_ignite->GetOrCreateCache<std::string, WorkerStatus>("WorkerStatus");
    workerStatusCache.Put(workerId, workerStatus);

    auto updatedStatus = workerStatusCache.Get(workerId);
    std::cerr << "updateWorkerStatus: retrieved status for " << updatedStatus.m_workerId << ": requests served: " << updatedStatus.m_requestsServed << std::endl;
}

void DataGridIgnite::removeWorkerStatus(const std::string& workerId)
{
}

std::string DataGridIgnite::readFromQueue(const std::string& topicName)
{
    return std::string();
}

void DataGridIgnite::sendWorkResponse(const std::string& gatewayId, const std::string& requestId, const std::string& response)
{
}

#if HAVE_API_MAXSAT
DataGrid::ErrorStatus DataGridIgnite::maxSatRulesetLock(const std::string& crsKey, const std::string& configCriteria)
{
    return ErrorStatus();
}

void DataGridIgnite::maxSatRulesetUnlock(const std::string& crsKey, const std::string& configCriteria)
{
}

DataGrid::ErrorStatus DataGridIgnite::maxSatRuleSetStore(const std::string& crsKey, const std::string& configCriteria, const std::string& maxSatRuleset)
{
    return ErrorStatus();
}

DataGrid::ErrorStatus DataGridIgnite::maxSatRuleSetQuery(const std::string& crsKey, std::map<std::string, std::string>& criteriaToMaxSatIdMap)
{
    return ErrorStatus();
}

DataGrid::ErrorStatus DataGridIgnite::maxSatRuleSetRetrieve(const std::string& rulesetId, std::string& maxSatRuleSet)
{
    return ErrorStatus();
}
#endif // HAVE_API_MAXSAT

void DataGridIgnite::storeCRS(const std::string& crsFileName, const std::string& crsData)
{
}

bool DataGridIgnite::downloadCRS(const std::string& crsFileName, const std::string& localFilePath)
{
    return false;
}
