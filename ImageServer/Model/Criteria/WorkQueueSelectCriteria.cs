using System;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class WorkQueueSelectCriteria : SelectCriteria
    {
        public WorkQueueSelectCriteria()
            : base("WorkQueue")
        {}

        public ISearchCondition<ServerEntityKey> Key
        {
            get
            {
                if (!SubCriteria.ContainsKey("Key"))
                {
                    SubCriteria["Key"] = new SearchCondition<ServerEntityKey>("Key");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["Key"];
            }
        }

        public ISearchCondition<ServerEntityKey> StudyStorageKey
        {
            get
            {
                if (!SubCriteria.ContainsKey("StudyStorageKey"))
                {
                    SubCriteria["StudyStorageKey"] = new SearchCondition<ServerEntityKey>("StudyStorageKey");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["StudyStorageKey"];
            }
        }
        public ISearchCondition<TypeEnum> TypeEnum
        {
            get
            {
                if (!SubCriteria.ContainsKey("TypeEnum"))
                {
                    SubCriteria["TypeEnum"] = new SearchCondition<TypeEnum>("TypeEnum");
                }
                return (ISearchCondition<TypeEnum>)SubCriteria["TypeEnum"];
            }
        }

        public ISearchCondition<StatusEnum> StatusEnum
        {
            get
            {
                if (!SubCriteria.ContainsKey("StatusEnum"))
                {
                    SubCriteria["StatusEnum"] = new SearchCondition<StatusEnum>("StatusEnum");
                }
                return (ISearchCondition<StatusEnum>)SubCriteria["StatusEnum"];
            }
        }
        public ISearchCondition<DateTime> ExpirationTime
        {
            get
            {
                if (!SubCriteria.ContainsKey("ExpirationTime"))
                {
                    SubCriteria["ExpirationTime"] = new SearchCondition<DateTime>("ExpirationTime");
                }
                return (ISearchCondition<DateTime>)SubCriteria["ExpirationTime"];
            }
        }
        public ISearchCondition<DateTime> ScheduledTime
        {
            get
            {
                if (!SubCriteria.ContainsKey("ScheduledTime"))
                {
                    SubCriteria["ScheduledTime"] = new SearchCondition<DateTime>("ScheduledTime");
                }
                return (ISearchCondition<DateTime>)SubCriteria["ScheduledTime"];
            }
        }
        public ISearchCondition<DateTime> InsertTime
        {
            get
            {
                if (!SubCriteria.ContainsKey("InsertTime"))
                {
                    SubCriteria["InsertTime"] = new SearchCondition<DateTime>("InsertTime");
                }
                return (ISearchCondition<DateTime>)SubCriteria["InsertTime"];
            }
        }

        public ISearchCondition<int> FailureCount
        {
            get
            {
                if (!SubCriteria.ContainsKey("FailureCount"))
                {
                    SubCriteria["FailureCount"] = new SearchCondition<int>("FailureCount");
                }
                return (ISearchCondition<int>)SubCriteria["FailureCount"];
            }
        }
    }
}
