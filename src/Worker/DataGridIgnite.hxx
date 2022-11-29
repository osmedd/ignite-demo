// @<COPYRIGHT>@
// ==================================================
// Copyright 2022.
// Siemens Product Lifecycle Management Software Inc.
// All Rights Reserved.
// ==================================================
// @<COPYRIGHT>@

/**
    @file

    DataGridIgnite implements the interface to Apache Ignite required by solver worker

*/

#pragma once
#include "DataGrid.hxx"
#include <ignite/ignition.h>
#include <ignite/cache/cache.h>

class DataGridIgnite : public DataGrid
{
public:
    DataGridIgnite();
    ~DataGridIgnite();

    std::string getWorkerId();
    std::string readFromQueue(const std::string& topicName);
    std::string readCurrentLogLevel();
    void storeCRS(const std::string& crsFileName, const std::string& crsData);
    bool downloadCRS(const std::string& crsFileName, const std::string& localFilePath);
    void updateWorkerStatus(const std::string& workerId, WorkerStatus& workerStatus);
    void removeWorkerStatus(const std::string& workerId);
    void sendWorkResponse(const std::string& gatewayId, const std::string& requestId, const std::string& response);
#if HAVE_API_MAXSAT
    ErrorStatus maxSatRulesetLock(const std::string& crsKey, const std::string& configCriteria);
    void maxSatRulesetUnlock(const std::string& crsKey, const std::string& configCriteria);
    ErrorStatus maxSatRuleSetStore(const std::string& crsKey, const std::string& configCriteria, const std::string& maxSatRuleset);
    ErrorStatus maxSatRuleSetQuery(const std::string& crsKey, std::map<std::string, std::string>& criteriaToMaxSatIdMap);
    ErrorStatus maxSatRuleSetRetrieve(const std::string& rulesetId, std::string& maxSatRuleSet);
#endif // HAVE_API_MAXSAT

private:
    std::shared_ptr<ignite::Ignite> m_ignite;
};
