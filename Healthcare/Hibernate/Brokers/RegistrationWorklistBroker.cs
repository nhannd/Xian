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

            public string Name;
            public object Value;
        }

        private const string _hqlSelectWorklist =       "select distinct o from CheckInProcedureStep cps";
        private const string _hqlSelectCount =          "select count(distinct o) from CheckInProcedureStep cps";
        private const string _hqlJoin =                 " join cps.RequestedProcedure rp" +
                                                        " join rp.Order o";
        private const string _hqlMainCondition =        " where cps.State = :cpsState" +
                                                        " and cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd";
        
        private const string _hqlMPSStartedSubQuery =   " and o in" +
                                                        " (select distinct o from ModalityProcedureStep mps join mps.RequestedProcedure rp join rp.Order o where" +
                                                        " (mps.State != :mpsState and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd))";

        private const string _hqlMPSNotStartedSubQuery= " and o not in" +
                                                        " (select distinct o from ModalityProcedureStep mps join mps.RequestedProcedure rp join rp.Order o where" +
                                                        " (mps.State != :mpsState and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd))";

        private const string _hqlWorklistSubQuery = " and rp.Type in" +
                                                    " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = :worklist)";

        private const string _hqlSelectProtocolWorklist     = "select distinct o from ProtocolProcedureStep cps";
        private const string _hqlSelectProtocolCount        = "select count(distinct o) from ProtocolProcedureStep cps";
        private const string _hqlProtocolStateCondition     = " where cps.State = :cpsState";
        private const string _hqlUnscheduledCondition       = " and rp.ScheduledStartTime is NULL";
        private const string _hqlDualProtocolStateCondition = " where (cps.State = :cpsState or cps.State = :cpsState2)";

        #region Query helpers

        private void AddMainQueryParameters(List<QueryParameter> parameters)
        {
            parameters.Add(new QueryParameter("cpsSchedulingStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("cpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1)));
        }

        private void AddSubQueryParameters(List<QueryParameter> parameters)
        {
            parameters.Add(new QueryParameter("mpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1)));
        }

        private void AddWorklistQueryAndParameters(ref string hqlQuery, List<QueryParameter> parameters, Worklist worklist)
        {
            if (worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }
        }

        private IList<WorklistItem> GetWorklist(string hqlQuery, List<QueryParameter> parameters)
        {
            List<WorklistItem> results = new List<WorklistItem>();

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object tuple in list)
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                results.Add(item);
            }

            return results;
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

        #endregion

        #region IRegistrationWorklistBroker Members

        #region Worklist

        public IList<WorklistItem> GetScheduledWorklist()
        {
            return GetScheduledWorklist(null);
        }

        public IList<WorklistItem> GetScheduledWorklist(RegistrationScheduledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SC.ToString()));
            AddMainQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCheckInWorklist()
        {
            return GetCheckInWorklist(null);
        }

        public IList<WorklistItem> GetCheckInWorklist(RegistrationCheckedInWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition, _hqlMPSNotStartedSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.IP.ToString()));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetInProgressWorklist()
        {
            return GetInProgressWorklist(null);
        }

        public IList<WorklistItem> GetInProgressWorklist(RegistrationInProgressWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition, _hqlMPSStartedSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.IP.ToString()));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCompletedWorklist()
        {
            return GetCompletedWorklist(null);
        }

        public IList<WorklistItem> GetCompletedWorklist(RegistrationCompletedWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.CM.ToString()));
            AddMainQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCancelledWorklist()
        {
            return GetCancelledWorklist(null);
        }

        public IList<WorklistItem> GetCancelledWorklist(RegistrationCancelledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.DC.ToString()));
            AddMainQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCompletedProtocolWorklist()
        {
            return GetCompletedProtocolWorklist(null);
        }

        public IList<WorklistItem> GetCompletedProtocolWorklist(RegistrationCompletedProtocolWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectProtocolWorklist, _hqlJoin, _hqlProtocolStateCondition, _hqlUnscheduledCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.CM.ToString()));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetSuspendedProtocolWorklist()
        {
            return GetSuspendedProtocolWorklist(null);
        }

        public IList<WorklistItem> GetSuspendedProtocolWorklist(RegistrationSuspendedProtocolWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectProtocolWorklist, _hqlJoin, _hqlProtocolStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SU.ToString()));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetPendingProtocolWorklist()
        {
            return GetPendingProtocolWorklist(null);
        }

        public IList<WorklistItem> GetPendingProtocolWorklist(RegistrationPendingProtocolWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectProtocolWorklist, _hqlJoin, _hqlDualProtocolStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("cpsState2", ActivityStatus.IP.ToString()));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklist(hqlQuery, parameters);
        }

        #endregion

        #region Worklist Counts

        public int GetScheduledWorklistCount()
        {
            return GetScheduledWorklistCount(null);
        }

        public int GetScheduledWorklistCount(RegistrationScheduledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SC.ToString()));
            AddMainQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCheckInWorklistCount()
        {
            return GetCheckInWorklistCount(null);
        }

        public int GetCheckInWorklistCount(RegistrationCheckedInWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition, _hqlMPSNotStartedSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.IP.ToString()));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetInProgressWorklistCount()
        {
            return GetInProgressWorklistCount(null);
        }

        public int GetInProgressWorklistCount(RegistrationInProgressWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition, _hqlMPSStartedSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.IP.ToString()));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCompletedWorklistCount()
        {
            return GetCompletedWorklistCount(null);
        }

        public int GetCompletedWorklistCount(RegistrationCompletedWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.CM.ToString()));
            AddMainQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCancelledWorklistCount()
        {
            return GetCancelledWorklistCount(null);
        }

        public int GetCancelledWorklistCount(RegistrationCancelledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.DC.ToString()));
            AddMainQueryParameters(parameters);

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCompletedProtocolWorklistCount()
        {
            return GetCompletedProtocolWorklistCount(null);
        }

        public int GetCompletedProtocolWorklistCount(RegistrationCompletedProtocolWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectProtocolCount, _hqlJoin, _hqlProtocolStateCondition, _hqlUnscheduledCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.CM.ToString()));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetSuspendedProtocolWorklistCount()
        {
            return GetSuspendedProtocolWorklistCount(null);
        }

        public int GetSuspendedProtocolWorklistCount(RegistrationSuspendedProtocolWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectProtocolCount, _hqlJoin, _hqlProtocolStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SU.ToString()));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetPendingProtocolWorklistCount()
        {
            return GetPendingProtocolWorklistCount(null);
        }

        public int GetPendingProtocolWorklistCount(RegistrationPendingProtocolWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectProtocolCount, _hqlJoin, _hqlDualProtocolStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("cpsState2", ActivityStatus.IP.ToString()));

            AddWorklistQueryAndParameters(ref hqlQuery, parameters, worklist);

            return GetWorklistCount(hqlQuery, parameters);
        }

        #endregion

        #region Query for worklist operation

        public IList<Order> GetOrdersForCheckIn(Patient patient)
        {
            List<Order> results = new List<Order>();
            string hqlQuery = "select distinct o from CheckInProcedureStep cps" +
                        " join cps.RequestedProcedure rp" +
                        " join rp.Order o" +
                        " join o.Patient p" +
                        _hqlMainCondition +
                        " and p = :patient";

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("patient", patient));
            AddMainQueryParameters(parameters);

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object tuple in list)
            {
                Order item = (Order)tuple;
                results.Add(item);
            }

            return results;
        }

        public int GetOrdersForCheckInCount(Patient patient)
        {
            string hqlQuery = "select count(distinct o) from CheckInProcedureStep cps" +
                        " join cps.RequestedProcedure rp" +
                        " join rp.Order o" +
                        " join o.Patient p" +
                        _hqlMainCondition +
                        " and p = :patient";

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("patient", patient));
            AddMainQueryParameters(parameters);

            IList list = DoQuery(hqlQuery, parameters);
            return (int)(long)list[0];
        }

        public IList<Order> GetOrdersForCancel(Patient patient)
        {
            List<Order> results = new List<Order>();
            string hqlQuery = "select distinct o from CheckInProcedureStep cps" +
                        " join cps.RequestedProcedure rp" +
                        " join rp.Order o" +
                        " join o.Patient p" +
                        " where (cps.State = :cpsState1 or cps.State = :cpsState2)" +
                        " and cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd" +
                        " and p = :patient" +
                        _hqlMPSNotStartedSubQuery;

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState1", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("cpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("patient", patient));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object tuple in list)
            {
                Order order = (Order)tuple;
                results.Add(order);
            }

            return results;
        }

        public int GetOrdersForCancelCount(Patient patient)
        {
            string hqlQuery = "select count(distinct o) from CheckInProcedureStep cps" +
                        " join cps.RequestedProcedure rp" +
                        " join rp.Order o" +
                        " join o.Patient p" +
                        " where (cps.State = :cpsState1 or cps.State = :cpsState2)" +
                        " and cps.Scheduling.StartTime between :cpsSchedulingStartTimeBegin and :cpsSchedulingStartTimeEnd" +
                        " and p = :patient" +
                        _hqlMPSNotStartedSubQuery;

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState1", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("cpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("patient", patient));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            IList list = DoQuery(hqlQuery, parameters);
            return (int)(long)list[0];
        }

        #endregion

        #region Query for Preview

        public IList<Order> GetOrdersForPatientPreview(Patient patient)
        {
            List<Order> results = new List<Order>();
            string hqlQuery = "select distinct o from ModalityProcedureStep mps" +
                        " join mps.RequestedProcedure rp" +
                        " join rp.Order o" +
                        " join o.Patient p" +
                        " where p = :patient" +
                        " and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd";

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("patient", patient));
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date.AddDays(-14)));
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(14)));

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object tuple in list)
            {
                Order item = (Order)tuple;
                results.Add(item);
            }

            return results;
        }

        #endregion

        public IList<WorklistItem> Search(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            hqlQuery.Append("select distinct o");
            hqlQuery.Append(" from Order o join o.Patient p join p.Profiles pp");

            HqlQuery query = new HqlQuery(hqlQuery.ToString());
            query.Page = page;

            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition(String.Format(
                    " (o.Status = '{0}' or o.Status = '{1}')", 
                    OrderStatus.SC, OrderStatus.IP), 
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
                query.Conditions.Add(new HqlCondition(String.Format(
                    " (o.Status = '{0}' or o.Status = '{1}')",
                    OrderStatus.SC, OrderStatus.IP), null));
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
    }
}
