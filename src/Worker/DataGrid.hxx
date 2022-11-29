// @<COPYRIGHT>@
// ==================================================
// Copyright 2019.
// Siemens Product Lifecycle Management Software Inc.
// All Rights Reserved.
// ==================================================
// @<COPYRIGHT>@

/**
    @file

    DataGrid implements the abstract interface to the datagrid required by solver worker

*/

#pragma once
#include <string>
#include <map>
#include "WorkerStatus.hxx"

class DataGrid
{
public:
    enum class ErrorStatus
    {
        OK,
        DataGridNotAvailable,
        EntryAlreadyExists,
        EntryNotFound
    };

    //virtual int64_t newWorkerId() = 0;
    virtual std::string getWorkerId() = 0;
    virtual std::string readFromQueue(const std::string& topicName) = 0;
    virtual std::string readCurrentLogLevel() = 0;
    virtual void storeCRS(const std::string& crsFileName, const std::string& crsData) = 0;
    virtual bool downloadCRS(const std::string& crsFileName, const std::string& localFilePath) = 0;
    virtual void updateWorkerStatus(const std::string& workerId, WorkerStatus& workerStatus) = 0;
    virtual void removeWorkerStatus(const std::string& workerId) = 0;
    virtual void sendWorkResponse(const std::string& gatewayId, const std::string& requestId, const std::string& response) = 0;
#if HAVE_API_MAXSAT
    virtual ErrorStatus maxSatRulesetLock(const std::string& crsKey, const std::string& configCriteria) = 0;
    virtual void maxSatRulesetUnlock(const std::string& crsKey, const std::string& configCriteria) = 0;
    virtual ErrorStatus maxSatRuleSetStore(const std::string& crsKey, const std::string& configCriteria, const std::string& maxSatRuleset) = 0;
    virtual ErrorStatus maxSatRuleSetQuery(const std::string& crsKey, std::map<std::string, std::string>& criteriaToMaxSatIdMap) = 0;
    virtual ErrorStatus maxSatRuleSetRetrieve(const std::string& rulesetId, std::string& maxSatRuleSet) = 0;
#endif // HAVE_API_MAXSAT
};
