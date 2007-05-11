using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;

using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ModalityWorklistBroker : Broker, IModalityWorklistBroker
    {
        private class QueryParameter
        {
            public QueryParameter(string name, object value)
            {
                this.Name = name;
                this.Value = value;
            }

            public string Name;
            public object Value;
        }

        private const string _hqlSelectWorklist = 
            "select mps, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority, rpt, spst, m" +
            " from ModalityProcedureStep mps";

        private const string _hqlSelectCount = 
            "select count(*)" +
            " from ModalityProcedureStep mps";

        private const string _hqlJoin =
            " join mps.Type spst" +
            " join mps.Modality m" +
            " join mps.RequestedProcedure rp" +
            " join rp.Type rpt" +
            " join rp.Order o" +
            " join o.DiagnosticService ds" +
            " join o.Visit v" +
            " join o.Patient p" +
            " join p.Profiles pp";

        private const string _hqlMainCondition = 
            " where mps.State = :mpsState" +
            " and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd";

        private const string _hqlMpsStateQuery =
            _hqlSelectWorklist + _hqlJoin + _hqlMainCondition;

        private const string _hqlMpsStateCountQuery =
            _hqlSelectCount + _hqlJoin + _hqlMainCondition;

        private const string _hqlScheduledSubCondition = 
            " and rp not in" +
            " (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp where" +
            " (cps.State = :cpsState and cps.StartTime between :cpsStartTimeBegin and :cpsStartTimeEnd))";

        private const string _hqlCheckInSubCondition = 
            " and rp in" +
            " (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp where" +
            " (cps.State = :cpsState and cps.StartTime between :cpsStartTimeBegin and :cpsStartTimeEnd))";

        #region IModalityWorklistBroker Members

        public IList<WorklistItem> GetScheduledWorklist()
        {
            string hqlQuery = String.Concat(_hqlMpsStateQuery, _hqlScheduledSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters("SC");
            AddSubQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCheckedInWorklist()
        {
            string hqlQuery = String.Concat(_hqlMpsStateQuery, _hqlCheckInSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters("SC");
            AddSubQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetInProgressWorklist()
        {
            return GetWorklistFromMpsState("IP");
        }

        public IList<WorklistItem> GetSuspendedWorklist()
        {
            return GetWorklistFromMpsState("SU");
        }

        public IList<WorklistItem> GetCompletedWorklist()
        {
            return GetWorklistFromMpsState("CM");
        }

        public IList<WorklistItem> GetCancelledWorklist()
        {
            return GetWorklistFromMpsState("DC");
        }

        public int GetScheduledWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlMpsStateCountQuery, _hqlScheduledSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters("SC");
            AddSubQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCheckedInWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlMpsStateCountQuery, _hqlCheckInSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters("SC");
            AddSubQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetInProgressWorklistCount()
        {
            return GetWorklistCountFromMpsState("IP");
        }

        public int GetSuspendedWorklistCount()
        {
            return GetWorklistCountFromMpsState("SU");
        }
        
        public int GetCompletedWorklistCount()
        {
            return GetWorklistCountFromMpsState("CM");
        }

        public int GetCancelledWorklistCount()
        {
            return GetWorklistCountFromMpsState("DC");
        }

        #endregion

        private IList<WorklistItem> GetWorklist(string hqlQuery, List<QueryParameter> parameters)
        {
            List<WorklistItem> worklistItems = new List<WorklistItem>();

            IList queryResults = DoQuery(hqlQuery, parameters);

            foreach (object[] tuple in queryResults)
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                worklistItems.Add(item);
            }

            return worklistItems;
        }

        private int GetWorklistCount(string hqlQuery, List<QueryParameter> parameters)
        {
            IList list = DoQuery(hqlQuery, parameters);
            return (int)list[0];
        }

        private IList DoQuery(string hqlQuery, List<QueryParameter> parameters)
        {
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            foreach (QueryParameter param in parameters)
            {
                query.SetParameter(param.Name, param.Value);
            }

            return query.List();
        }

        #region Basic query by MPS State helpers

        private List<QueryParameter> BaseMpsStateParameters(string state)
        {
            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", state));
            AddMpsDateRangeQueryParameters(parameters);

            return parameters;
        }

        private IList<WorklistItem> GetWorklistFromMpsState(string state)
        {
            return GetWorklist(_hqlMpsStateQuery, BaseMpsStateParameters(state));
        }

        private int GetWorklistCountFromMpsState(string state)
        {
            return GetWorklistCount(_hqlMpsStateCountQuery, BaseMpsStateParameters(state));
        }

        #endregion

        #region Extended query helpers

        private void AddMpsDateRangeQueryParameters(List<QueryParameter> parameters)
        {
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1)));
        }

        private void AddSubQueryParameters(List<QueryParameter> parameters)
        {
            parameters.Add(new QueryParameter("cpsState", "IP"));
            parameters.Add(new QueryParameter("cpsStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("cpsStartTimeEnd", Platform.Time.Date.AddDays(1)));
        }

        #endregion

    }
}
