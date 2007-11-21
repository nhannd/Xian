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

            public readonly string Name;
            public readonly object Value;
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
            " and (rp.ProcedureCheckIn is NULL or rp.ProcedureCheckIn.CheckInTime is NULL)";

        private const string _hqlCheckedInSubCondition =
            " and rp.ProcedureCheckIn is not NULL and rp.ProcedureCheckIn.CheckInTime is not NULL and rp.ProcedureCheckIn.CheckOutTime is NULL";

        private const string _hqlUndocumentedSubCondition =
            " and rp in" +
            " (select rp from DocumentationProcedureStep dps join dps.RequestedProcedure rp where" +
            " (dps.State = :dpsState and dps.StartTime between :dpsStartTimeBegin and :dpsStartTimeEnd))";

        private const string _hqlWorklistSubQuery = 
            " and rp.Type in" +
            " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = :worklist)";

        abstract class WorklistHelper
        {
            public string WorklistQuery;
            public string CountQuery;
            public readonly List<QueryParameter> Parameters = new List<QueryParameter>();

            protected void AddWorklistQueryAndParameters(Worklist worklist)
            {
                if (worklist != null)
                {
                    this.WorklistQuery += _hqlWorklistSubQuery;
                    this.CountQuery += _hqlWorklistSubQuery;
                    this.Parameters.Add(new QueryParameter("worklist", worklist));
                }
            }

            protected void AddBaseMpsStateParameters(string state)
            {
                this.Parameters.Add(new QueryParameter("mpsState", state));
                AddMpsDateRangeQueryParameters();
            }

            protected void AddMpsDateRangeQueryParameters()
            {
                this.Parameters.Add(new QueryParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date));
                this.Parameters.Add(new QueryParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1)));
            }
        }

        class ScheduledWorklistHelper : WorklistHelper
        {
            public ScheduledWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlMpsStateQuery, _hqlScheduledSubCondition);
                this.CountQuery = string.Concat(_hqlMpsStateCountQuery, _hqlScheduledSubCondition);
                AddBaseMpsStateParameters(ActivityStatus.SC.ToString());
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class CheckedInWorklistHelper : WorklistHelper
        {
            public CheckedInWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlMpsStateQuery, _hqlCheckedInSubCondition);
                this.CountQuery = string.Concat(_hqlMpsStateCountQuery, _hqlCheckedInSubCondition);
                AddBaseMpsStateParameters(ActivityStatus.SC.ToString());
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class UnDocumentedWorklistHelper : WorklistHelper
        {
            public UnDocumentedWorklistHelper(Worklist worklist)
            {
                string hqlMpsStatelessQuery = " where (mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd)";

                this.WorklistQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, hqlMpsStatelessQuery, _hqlUndocumentedSubCondition);
                this.CountQuery = String.Concat(_hqlSelectCount, _hqlJoin, hqlMpsStatelessQuery, _hqlUndocumentedSubCondition);
                AddMpsDateRangeQueryParameters();
                this.Parameters.Add(new QueryParameter("dpsState", ActivityStatus.IP.ToString()));
                this.Parameters.Add(new QueryParameter("dpsStartTimeBegin", Platform.Time.Date));
                this.Parameters.Add(new QueryParameter("dpsStartTimeEnd", Platform.Time.Date.AddDays(1)));
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class MpsStateWorklistHelper : WorklistHelper
        {
            public MpsStateWorklistHelper(string state, Worklist worklist)
            {
                this.WorklistQuery = _hqlMpsStateQuery;
                this.CountQuery = _hqlMpsStateCountQuery;
                AddBaseMpsStateParameters(state);
                AddWorklistQueryAndParameters(worklist);
            }
        }

        #region Worklist query helpers

        private IList<WorklistItem> GetWorklist(WorklistHelper helper)
        {
            List<WorklistItem> results = new List<WorklistItem>();

            IList list = DoQuery(helper.WorklistQuery, helper.Parameters);
            foreach (object[] tuple in list)
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                results.Add(item);
            }

            return results;
        }

        private int GetWorklistCount(WorklistHelper helper)
        {
            IList list = DoQuery(helper.CountQuery, helper.Parameters);
            return (int)(long)list[0];
        }

        private IList DoQuery(string hqlQuery, IEnumerable<QueryParameter> parameters)
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

        #endregion

        #region IModalityWorklistBroker Members

        #region Worklists

        public IList<WorklistItem> GetScheduledWorklist()
        {
            return GetScheduledWorklist(null);
        }

        public IList<WorklistItem> GetScheduledWorklist(TechnologistScheduledWorklist worklist)
        {
            return GetWorklist(new ScheduledWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetCheckedInWorklist()
        {
            return GetCheckedInWorklist(null);
        }

        public IList<WorklistItem> GetCheckedInWorklist(TechnologistCheckedInWorklist worklist)
        {
            return GetWorklist(new CheckedInWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetUndocumentedWorklist()
        {
            return GetUndocumentedWorklist(null);
        }

        public IList<WorklistItem> GetUndocumentedWorklist(TechnologistUndocumentedWorklist worklist)
        {
            return GetWorklist(new UnDocumentedWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetInProgressWorklist()
        {
            return GetInProgressWorklist(null);
        }

        public IList<WorklistItem> GetInProgressWorklist(TechnologistInProgressWorklist worklist)
        {
            return GetWorklist(new MpsStateWorklistHelper(ActivityStatus.IP.ToString(), worklist));
        }

        public IList<WorklistItem> GetSuspendedWorklist()
        {
            return GetSuspendedWorklist(null);
        }

        public IList<WorklistItem> GetSuspendedWorklist(TechnologistSuspendedWorklist worklist)
        {
            return GetWorklist(new MpsStateWorklistHelper(ActivityStatus.SU.ToString(), worklist));
        }

        public IList<WorklistItem> GetCompletedWorklist()
        {
            return GetCompletedWorklist(null);
        }

        public IList<WorklistItem> GetCompletedWorklist(TechnologistCompletedWorklist worklist)
        {
            return GetWorklist(new MpsStateWorklistHelper(ActivityStatus.CM.ToString(), worklist));
        }

        public IList<WorklistItem> GetCancelledWorklist()
        {
            return GetCancelledWorklist(null);
        }

        public IList<WorklistItem> GetCancelledWorklist(TechnologistCancelledWorklist worklist)
        {
            return GetWorklist(new MpsStateWorklistHelper(ActivityStatus.DC.ToString(), worklist));
        }

        #endregion

        #region Counts

        public int GetScheduledWorklistCount()
        {
            return GetScheduledWorklistCount(null);
        }

        public int GetScheduledWorklistCount(TechnologistScheduledWorklist worklist)
        {
            return GetWorklistCount(new ScheduledWorklistHelper(worklist));
        }

        public int GetCheckedInWorklistCount()
        {
            return GetCheckedInWorklistCount(null);
        }

        public int GetCheckedInWorklistCount(TechnologistCheckedInWorklist worklist)
        {
            return GetWorklistCount(new CheckedInWorklistHelper(worklist));
        }

        public int GetUndocumentedWorklistCount()
        {
            return GetUndocumentedWorklistCount(null);
        }

        public int GetUndocumentedWorklistCount(TechnologistUndocumentedWorklist worklist)
        {
            return GetWorklistCount(new UnDocumentedWorklistHelper(worklist));
        }

        public int GetInProgressWorklistCount()
        {
            return GetInProgressWorklistCount(null);
        }

        public int GetInProgressWorklistCount(TechnologistInProgressWorklist worklist)
        {
            return GetWorklistCount(new MpsStateWorklistHelper(ActivityStatus.IP.ToString(), worklist));
        }

        public int GetSuspendedWorklistCount()
        {
            return GetSuspendedWorklistCount(null);
        }

        public int GetSuspendedWorklistCount(TechnologistSuspendedWorklist worklist)
        {
            return GetWorklistCount(new MpsStateWorklistHelper(ActivityStatus.SU.ToString(), worklist));
        }

        public int GetCompletedWorklistCount()
        {
            return GetCompletedWorklistCount(null);
        }

        public int GetCompletedWorklistCount(TechnologistCompletedWorklist worklist)
        {
            return GetWorklistCount(new MpsStateWorklistHelper(ActivityStatus.CM.ToString(), worklist));
        }

        public int GetCancelledWorklistCount()
        {
            return GetCancelledWorklistCount(null);
        }

        public int GetCancelledWorklistCount(TechnologistCancelledWorklist worklist)
        {
            return GetWorklistCount(new MpsStateWorklistHelper(ActivityStatus.DC.ToString(), worklist));
        }

        #endregion

        #region Search

        public IList<WorklistItem> Search(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            hqlQuery.Append(_hqlSelectWorklist);
            hqlQuery.Append(_hqlJoin);

            HqlQuery query = new HqlQuery(hqlQuery.ToString());
            query.Page = page;

            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("mps.State in (?, ?)", ActivityStatus.SC, ActivityStatus.IP));
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
            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("mps.State in (?, ?)", ActivityStatus.SC, ActivityStatus.IP));
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

            return (int) ExecuteHqlUnique<long>(query);
        }

        #endregion

        #endregion


    }
}
