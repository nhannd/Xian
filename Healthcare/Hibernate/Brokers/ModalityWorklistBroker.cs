#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Workflow;
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
            "select mps, o, p, pp, rpt, spst, m, v.PatientClass, ds.Name" +
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

        private const string _hqlUndocumentedSubCondition =
            " and rp in" +
            " (select rp from DocumentationProcedureStep dps join dps.RequestedProcedure rp where" +
            " (dps.State = :dpsState and dps.StartTime between :dpsStartTimeBegin and :dpsStartTimeEnd))";

        private const string _hqlWorklistSubQuery = 
            " and rp.Type in" +
            " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = :worklist)";

        #region IModalityWorklistBroker Members

        #region Worklists

        public IList<WorklistItem> GetScheduledWorklist()
        {
            return GetScheduledWorklist(null);
        }

        public IList<WorklistItem> GetScheduledWorklist(TechnologistScheduledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlMpsStateQuery, _hqlScheduledSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters(ActivityStatus.SC.ToString());
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCheckedInWorklist()
        {
            return GetCheckedInWorklist(null);
        }

        public IList<WorklistItem> GetCheckedInWorklist(TechnologistCheckedInWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlMpsStateQuery, _hqlCheckInSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters(ActivityStatus.SC.ToString());
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetUndocumentedWorklist()
        {
            return GetUndocumentedWorklist(null);
        }

        public IList<WorklistItem> GetUndocumentedWorklist(TechnologistUndocumentedWorklist worklist)
        {
            string hqlMpsStatelessQuery = _hqlSelectWorklist + _hqlJoin + 
                " where (mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd)";

            string hqlQuery = String.Concat(hqlMpsStatelessQuery, _hqlUndocumentedSubCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            AddMpsDateRangeQueryParameters(parameters);

            parameters.Add(new QueryParameter("dpsState", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("dpsStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("dpsStartTimeEnd", Platform.Time.Date.AddDays(1)));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetInProgressWorklist()
        {
            return GetInProgressWorklist(null);
        }

        public IList<WorklistItem> GetInProgressWorklist(TechnologistInProgressWorklist worklist)
        {
            return GetWorklistFromMpsState(ActivityStatus.IP.ToString(), worklist);
        }

        public IList<WorklistItem> GetSuspendedWorklist()
        {
            return GetSuspendedWorklist(null);
        }

        public IList<WorklistItem> GetSuspendedWorklist(TechnologistSuspendedWorklist worklist)
        {
            return GetWorklistFromMpsState(ActivityStatus.SU.ToString(), worklist);
        }

        public IList<WorklistItem> GetCompletedWorklist()
        {
            return GetCompletedWorklist(null);
        }

        public IList<WorklistItem> GetCompletedWorklist(TechnologistCompletedWorklist worklist)
        {
            return GetWorklistFromMpsState(ActivityStatus.CM.ToString(), worklist);
        }

        public IList<WorklistItem> GetCancelledWorklist()
        {
            return GetCancelledWorklist(null);
        }

        public IList<WorklistItem> GetCancelledWorklist(TechnologistCancelledWorklist worklist)
        {
            return GetWorklistFromMpsState(ActivityStatus.DC.ToString(), worklist);
        }

        #endregion

        #region Counts

        public int GetScheduledWorklistCount()
        {
            return GetScheduledWorklistCount(null);
        }

        public int GetScheduledWorklistCount(TechnologistScheduledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlMpsStateCountQuery, _hqlScheduledSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters("SC");
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCheckedInWorklistCount()
        {
            return GetCheckedInWorklistCount(null);
        }

        public int GetCheckedInWorklistCount(TechnologistCheckedInWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlMpsStateCountQuery, _hqlCheckInSubCondition);

            List<QueryParameter> parameters = BaseMpsStateParameters("SC");
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetUndocumentedWorklistCount()
        {
            return GetUndocumentedWorklistCount(null);
        }

        public int GetUndocumentedWorklistCount(TechnologistUndocumentedWorklist worklist)
        {
            string hqlMpsStatelessQuery = _hqlSelectCount + _hqlJoin +
                " where (mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd)";

            string hqlQuery = String.Concat(hqlMpsStatelessQuery, _hqlUndocumentedSubCondition);

            //List<QueryParameter> parameters = BaseMpsStateParameters(ActivityStatus.IP.ToString());
            List<QueryParameter> parameters = new List<QueryParameter>();
            AddMpsDateRangeQueryParameters(parameters);

            parameters.Add(new QueryParameter("dpsState", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("dpsStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("dpsStartTimeEnd", Platform.Time.Date.AddDays(1)));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetInProgressWorklistCount()
        {
            return GetInProgressWorklistCount(null);
        }

        public int GetInProgressWorklistCount(TechnologistInProgressWorklist worklist)
        {
            return GetWorklistCountFromMpsState("IP", worklist);
        }

        public int GetSuspendedWorklistCount()
        {
            return GetSuspendedWorklistCount(null);
        }

        public int GetSuspendedWorklistCount(TechnologistSuspendedWorklist worklist)
        {
            return GetWorklistCountFromMpsState("SU", worklist);
        }

        public int GetCompletedWorklistCount()
        {
            return GetCompletedWorklistCount(null);
        }

        public int GetCompletedWorklistCount(TechnologistCompletedWorklist worklist)
        {
            return GetWorklistCountFromMpsState("CM", worklist);
        }

        public int GetCancelledWorklistCount()
        {
            return GetCancelledWorklistCount(null);
        }

        public int GetCancelledWorklistCount(TechnologistCancelledWorklist worklist)
        {
            return GetWorklistCountFromMpsState("DC", worklist);
        }

        #endregion

        public IList<WorklistItem> Search(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            hqlQuery.Append(_hqlSelectWorklist);
            hqlQuery.Append(_hqlJoin);

            HqlQuery query = new HqlQuery(hqlQuery.ToString());
            query.Page = page;

            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition(String.Format(
                    " rp in (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp" +
                    " where (cps.State != '{0}' and cps.State != '{1}'))", 
                    ActivityStatus.CM, ActivityStatus.DC), 
                    new object[] {} ));
            }

            HqlOr or = new HqlOr();
            foreach (WorklistItemSearchCriteria c in where)
            {
                HqlAnd and = new HqlAnd();

                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", c.Order));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("pp", c.PatientProfile));

                if (and.Conditions.Count > 0)
                    or.Conditions.Add(and);

                query.Sorts.AddRange(HqlSort.FromSearchCriteria("o", c.Order));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("pp", c.PatientProfile));
            }

            if (or.Conditions.Count > 0)
                query.Conditions.Add(or);

            IList<object> list = ExecuteHql<object>(query);
            List<WorklistItem> results = new List<WorklistItem>();
            foreach (object[] tuple in list)
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                results.Add(item);
            }

            return results;
        }

        public int SearchCount(WorklistItemSearchCriteria[] where, bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            hqlQuery.Append(_hqlSelectCount);
            hqlQuery.Append(_hqlJoin);

            HqlQuery query = new HqlQuery(hqlQuery.ToString());

            HqlOr or = new HqlOr();
            foreach (WorklistItemSearchCriteria c in where)
            {
                HqlAnd and = new HqlAnd();

                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", c.Order));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("pp", c.PatientProfile));

                if (and.Conditions.Count > 0)
                    or.Conditions.Add(and);

                query.Sorts.AddRange(HqlSort.FromSearchCriteria("o", c.Order));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("pp", c.PatientProfile));
            }

            if (or.Conditions.Count > 0)
                query.Conditions.Add(or);

            return (int) ExecuteHqlUnique<long>(query);
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
            return (int)(long)list[0];
        }

        private IList DoQuery(string hqlQuery, List<QueryParameter> parameters)
        {
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);

            if (parameters != null)
            {
                foreach (QueryParameter param in parameters)
                {
                    query.SetParameter(param.Name, param.Value);
                }
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

        private IList<WorklistItem> GetWorklistFromMpsState(string state, Worklist worklist)
        {
            string hqlQuery = _hqlMpsStateQuery;
            List<QueryParameter> parameters = BaseMpsStateParameters(state);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        private int GetWorklistCountFromMpsState(string state, Worklist worklist)
        {
            string hqlQuery = _hqlMpsStateCountQuery;
            List<QueryParameter> parameters = BaseMpsStateParameters(state);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
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
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("cpsStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("cpsStartTimeEnd", Platform.Time.Date.AddDays(1)));
        }

        #endregion

        private void AddWorklistQueryAndParameters(ref string hqlQuery, List<QueryParameter> parameters, Worklist worklist)
        {
            if (worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }
        }
    }
}
