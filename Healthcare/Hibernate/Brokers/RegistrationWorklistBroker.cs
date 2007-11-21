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
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Workflow;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class RegistrationWorklistBroker : Broker, IRegistrationWorklistBroker
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

        private const string _hqlSelectWorklist =       "select distinct o from RequestedProcedure rp";
        private const string _hqlSelectCount =          "select count(distinct o) from RequestedProcedure rp";
        private const string _hqlJoin =                 " join rp.Order o";

        private const string _hqlScheduledCondition =   " where rp.ScheduledStartTime between :orderScheduleStartTimeBegin and :orderScheduleStartTimeEnd";
        private const string _hqlOrderStatusCondition = " and o.Status = :orderStatus";
        private const string _hqlNotCheckedInCondition = " and (rp.ProcedureCheckIn is NULL or rp.ProcedureCheckIn.CheckInTime is NULL)";
        private const string _hqlCheckedInCondition =   " and rp.ProcedureCheckIn is not NULL and rp.ProcedureCheckIn.CheckInTime is not NULL and rp.ProcedureCheckIn.CheckOutTime is NULL";
        private const string _hqlCheckedOutCondition =  " and rp.ProcedureCheckIn is not NULL and rp.ProcedureCheckIn.CheckOutTime is not NULL";

        private const string _hqlScheduledWorklist =    _hqlScheduledCondition + _hqlOrderStatusCondition + _hqlNotCheckedInCondition;
        private const string _hqlCheckedInWorklist =    _hqlScheduledCondition + _hqlOrderStatusCondition + _hqlCheckedInCondition;
        private const string _hqlInProgressWorklist =   _hqlScheduledCondition + _hqlOrderStatusCondition;
        private const string _hqlCompletedWorklist =    _hqlScheduledCondition + " and (o.Status = :orderStatus or o.Status = :orderStatus2" + _hqlCheckedOutCondition + ")";
        private const string _hqlCancelledWorklist =    _hqlScheduledCondition + _hqlOrderStatusCondition;
                
        private const string _hqlWorklistSubQuery =         " and rp.Type in" +
                                                            " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = :worklist)";

        private const string _hqlSelectProtocolWorklist     = "select distinct o from ProtocolProcedureStep cps";
        private const string _hqlSelectProtocolCount        = "select count(distinct o) from ProtocolProcedureStep cps";
        private const string _hqlProtocolJoin               = " join cps.RequestedProcedure rp" +
                                                              " join rp.Order o";
        private const string _hqlUnscheduledCondition =       " and rp.ScheduledStartTime is NULL";
        private const string _hqlProtocolStateCondition =     " where cps.State = :cpsState";
        private const string _hqlDualProtocolStateCondition = " where (cps.State = :cpsState or cps.State = :cpsState2)";

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

            protected void AddOrderStatusParameters(string state)
            {
                this.Parameters.Add(new QueryParameter("orderStatus", state));
                AddOrderScheduledDateRangeQueryParameters();
            }

            protected void AddDualOrderStatusParameters(string state, string state2)
            {
                this.Parameters.Add(new QueryParameter("orderStatus", state));
                this.Parameters.Add(new QueryParameter("orderStatus2", state2));
                AddOrderScheduledDateRangeQueryParameters();
            }

            private void AddOrderScheduledDateRangeQueryParameters()
            {
                this.Parameters.Add(new QueryParameter("orderScheduleStartTimeBegin", Platform.Time.Date));
                this.Parameters.Add(new QueryParameter("orderScheduleStartTimeEnd", Platform.Time.Date.AddDays(1)));
            }
        }

        class ScheduledWorklistHelper : WorklistHelper
        {
            public ScheduledWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectWorklist, _hqlJoin, _hqlScheduledWorklist);
                this.CountQuery = string.Concat(_hqlSelectCount, _hqlJoin, _hqlScheduledWorklist);
                AddOrderStatusParameters(OrderStatus.SC.ToString());
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class CheckedInWorklistHelper : WorklistHelper
        {
            public CheckedInWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectWorklist, _hqlJoin, _hqlCheckedInWorklist);
                this.CountQuery = string.Concat(_hqlSelectCount, _hqlJoin, _hqlCheckedInWorklist);
                AddOrderStatusParameters(OrderStatus.SC.ToString());
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class InProgressWorklistHelper : WorklistHelper
        {
            public InProgressWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectWorklist, _hqlJoin, _hqlInProgressWorklist);
                this.CountQuery = string.Concat(_hqlSelectCount, _hqlJoin, _hqlInProgressWorklist);
                AddOrderStatusParameters(OrderStatus.IP.ToString());
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class CompletedWorklistHelper : WorklistHelper
        {
            public CompletedWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectWorklist, _hqlJoin, _hqlCompletedWorklist);
                this.CountQuery = string.Concat(_hqlSelectCount, _hqlJoin, _hqlCompletedWorklist);
                AddDualOrderStatusParameters(OrderStatus.CM.ToString(), OrderStatus.IP.ToString());
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class CancelledWorklistHelper : WorklistHelper
        {
            public CancelledWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectWorklist, _hqlJoin, _hqlCancelledWorklist);
                this.CountQuery = string.Concat(_hqlSelectCount, _hqlJoin, _hqlCancelledWorklist);
                AddOrderStatusParameters(OrderStatus.DC.ToString());
                AddWorklistQueryAndParameters(worklist);
            }
        }

        class CompletedProtocolWorklistHelper : WorklistHelper
        {
            public CompletedProtocolWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectProtocolWorklist, _hqlProtocolJoin, _hqlProtocolStateCondition, _hqlUnscheduledCondition);
                this.CountQuery = string.Concat(_hqlSelectProtocolCount, _hqlProtocolJoin, _hqlProtocolStateCondition, _hqlUnscheduledCondition);
                this.Parameters.Add(new QueryParameter("cpsState", ActivityStatus.IP.ToString()));
                this.Parameters.Add(new QueryParameter("cpsState", ActivityStatus.CM.ToString()));
                this.AddWorklistQueryAndParameters(worklist);
            }
        }

        class SuspendedProtocolWorklistHelper : WorklistHelper
        {
            public SuspendedProtocolWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectProtocolWorklist, _hqlProtocolJoin, _hqlProtocolStateCondition);
                this.CountQuery = string.Concat(_hqlSelectProtocolCount, _hqlProtocolJoin, _hqlProtocolStateCondition);
                this.Parameters.Add(new QueryParameter("cpsState", ActivityStatus.SU.ToString()));
                this.AddWorklistQueryAndParameters(worklist);
            }
        }

        class PendingProtocolWorklistHelper : WorklistHelper
        {
            public PendingProtocolWorklistHelper(Worklist worklist)
            {
                this.WorklistQuery = string.Concat(_hqlSelectProtocolWorklist, _hqlProtocolJoin, _hqlDualProtocolStateCondition);
                this.CountQuery = string.Concat(_hqlSelectProtocolCount, _hqlProtocolJoin, _hqlDualProtocolStateCondition);
                this.Parameters.Add(new QueryParameter("cpsState", ActivityStatus.SC.ToString()));
                this.Parameters.Add(new QueryParameter("cpsState2", ActivityStatus.IP.ToString()));
                this.AddWorklistQueryAndParameters(worklist);
            }
        }

        class GetOrdersForCheckInHelper : WorklistHelper
        {
            public GetOrdersForCheckInHelper(Patient patient)
            {
                this.WorklistQuery = string.Concat(_hqlSelectWorklist, _hqlJoin, " join o.Patient p", _hqlScheduledCondition, _hqlOrderStatusCondition, _hqlNotCheckedInCondition, " and p = :patient");
                this.CountQuery = string.Concat(_hqlSelectCount, _hqlJoin, " join o.Patient p", _hqlScheduledCondition, _hqlOrderStatusCondition, _hqlNotCheckedInCondition, " and p = :patient");
                AddOrderStatusParameters(OrderStatus.SC.ToString());
                this.Parameters.Add(new QueryParameter("patient", patient));
            }
        }

        class GetOrdersForCancelHelper : WorklistHelper
        {
            public GetOrdersForCancelHelper(Patient patient)
            {
                this.WorklistQuery = string.Concat(_hqlSelectWorklist, _hqlJoin, " join o.Patient p", _hqlScheduledCondition, _hqlOrderStatusCondition, " and p = :patient");
                this.CountQuery = string.Concat(_hqlSelectCount, _hqlJoin, " join o.Patient p", _hqlScheduledCondition, _hqlOrderStatusCondition, " and p = :patient");
                AddOrderStatusParameters(OrderStatus.SC.ToString());
                this.Parameters.Add(new QueryParameter("patient", patient));
            }
        }

        #region Worklist query helpers

        private IList<WorklistItem> GetWorklist(WorklistHelper helper)
        {
            List<WorklistItem> results = new List<WorklistItem>();

            IList list = DoQuery(helper.WorklistQuery, helper.Parameters);
            foreach (object tuple in list)
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                results.Add(item);
            }

            return results;
        }

        private int GetWorklistCount(WorklistHelper helper)
        {
            IList list = DoQuery(helper.CountQuery, helper.Parameters);
            return (int) (long) list[0];
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

        #region IRegistrationWorklistBroker Members

        #region Worklist

        public IList<WorklistItem> GetScheduledWorklist()
        {
            return GetScheduledWorklist(null);
        }

        public IList<WorklistItem> GetScheduledWorklist(RegistrationScheduledWorklist worklist)
        {
            return GetWorklist(new ScheduledWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetCheckInWorklist()
        {
            return GetCheckInWorklist(null);
        }

        public IList<WorklistItem> GetCheckInWorklist(RegistrationCheckedInWorklist worklist)
        {
            return GetWorklist(new CheckedInWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetInProgressWorklist()
        {
            return GetInProgressWorklist(null);
        }

        public IList<WorklistItem> GetInProgressWorklist(RegistrationInProgressWorklist worklist)
        {
            return GetWorklist(new InProgressWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetCompletedWorklist()
        {
            return GetCompletedWorklist(null);
        }

        public IList<WorklistItem> GetCompletedWorklist(RegistrationCompletedWorklist worklist)
        {
            return GetWorklist(new CompletedWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetCancelledWorklist()
        {
            return GetCancelledWorklist(null);
        }

        public IList<WorklistItem> GetCancelledWorklist(RegistrationCancelledWorklist worklist)
        {
            return GetWorklist(new CancelledWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetCompletedProtocolWorklist()
        {
            return GetCompletedProtocolWorklist(null);
        }

        public IList<WorklistItem> GetCompletedProtocolWorklist(RegistrationCompletedProtocolWorklist worklist)
        {
            return GetWorklist(new CompletedProtocolWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetSuspendedProtocolWorklist()
        {
            return GetSuspendedProtocolWorklist(null);
        }

        public IList<WorklistItem> GetSuspendedProtocolWorklist(RegistrationSuspendedProtocolWorklist worklist)
        {
            return GetWorklist(new SuspendedProtocolWorklistHelper(worklist));
        }

        public IList<WorklistItem> GetPendingProtocolWorklist()
        {
            return GetPendingProtocolWorklist(null);
        }

        public IList<WorklistItem> GetPendingProtocolWorklist(RegistrationPendingProtocolWorklist worklist)
        {
            return GetWorklist(new PendingProtocolWorklistHelper(worklist));
        }

        #endregion

        #region Counts

        public int GetScheduledWorklistCount()
        {
            return GetScheduledWorklistCount(null);
        }

        public int GetScheduledWorklistCount(RegistrationScheduledWorklist worklist)
        {
            return GetWorklistCount(new ScheduledWorklistHelper(worklist));
        }

        public int GetCheckInWorklistCount()
        {
            return GetCheckInWorklistCount(null);
        }

        public int GetCheckInWorklistCount(RegistrationCheckedInWorklist worklist)
        {
            return GetWorklistCount(new CheckedInWorklistHelper(worklist));
        }

        public int GetInProgressWorklistCount()
        {
            return GetInProgressWorklistCount(null);
        }

        public int GetInProgressWorklistCount(RegistrationInProgressWorklist worklist)
        {
            return GetWorklistCount(new InProgressWorklistHelper(worklist));
        }

        public int GetCompletedWorklistCount()
        {
            return GetCompletedWorklistCount(null);
        }

        public int GetCompletedWorklistCount(RegistrationCompletedWorklist worklist)
        {
            return GetWorklistCount(new CompletedWorklistHelper(worklist));
        }

        public int GetCancelledWorklistCount()
        {
            return GetCancelledWorklistCount(null);
        }

        public int GetCancelledWorklistCount(RegistrationCancelledWorklist worklist)
        {
            return GetWorklistCount(new CancelledWorklistHelper(worklist));
        }

        public int GetCompletedProtocolWorklistCount()
        {
            return GetCompletedProtocolWorklistCount(null);
        }

        public int GetCompletedProtocolWorklistCount(RegistrationCompletedProtocolWorklist worklist)
        {
            return GetWorklistCount(new CompletedProtocolWorklistHelper(worklist));
        }

        public int GetSuspendedProtocolWorklistCount()
        {
            return GetSuspendedProtocolWorklistCount(null);
        }

        public int GetSuspendedProtocolWorklistCount(RegistrationSuspendedProtocolWorklist worklist)
        {
            return GetWorklistCount(new SuspendedProtocolWorklistHelper(worklist));
        }

        public int GetPendingProtocolWorklistCount()
        {
            return GetPendingProtocolWorklistCount(null);
        }

        public int GetPendingProtocolWorklistCount(RegistrationPendingProtocolWorklist worklist)
        {
            return GetWorklistCount(new PendingProtocolWorklistHelper(worklist));
        }

        #endregion

        #region Query for worklist operation

        public IList<Order> GetOrdersForCheckIn(Patient patient)
        {
            GetOrdersForCheckInHelper helper = new GetOrdersForCheckInHelper(patient);

            List<Order> results = new List<Order>();
            IList list = DoQuery(helper.WorklistQuery, helper.Parameters);
            foreach (object tuple in list)
            {
                Order item = (Order)tuple;
                results.Add(item);
            }

            return results;
        }

        public int GetOrdersForCheckInCount(Patient patient)
        {
            GetOrdersForCheckInHelper helper = new GetOrdersForCheckInHelper(patient);
            IList list = DoQuery(helper.CountQuery, helper.Parameters);
            return (int)(long)list[0];
        }

        public IList<Order> GetOrdersForCancel(Patient patient)
        {
            GetOrdersForCancelHelper helper = new GetOrdersForCancelHelper(patient);

            List<Order> results = new List<Order>();
            IList list = DoQuery(helper.WorklistQuery, helper.Parameters);
            foreach (object tuple in list)
            {
                Order order = (Order)tuple;
                results.Add(order);
            }

            return results;
        }

        public int GetOrdersForCancelCount(Patient patient)
        {
            GetOrdersForCancelHelper helper = new GetOrdersForCancelHelper(patient);
            IList list = DoQuery(helper.CountQuery, helper.Parameters);
            return (int)(long)list[0];
        }

        #endregion

        #region Search

        public IList<WorklistItem> Search(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            hqlQuery.Append("select distinct o");
            hqlQuery.Append(" from Order o join o.Patient p join p.Profiles pp");

            HqlQuery query = new HqlQuery(hqlQuery.ToString());
            query.Page = page;

            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("(o.Status in (?, ?))", OrderStatus.SC, OrderStatus.IP));
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
            foreach (object tuple in list)
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                results.Add(item);
            }

            return results;
        }

        public int SearchCount(WorklistItemSearchCriteria[] where, bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            hqlQuery.Append("select distinct o");
            hqlQuery.Append(" from Order o join o.Patient p join p.Profiles pp");

            HqlQuery query = new HqlQuery(hqlQuery.ToString());

            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("(o.Status in (?, ?))", OrderStatus.SC, OrderStatus.IP));
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
