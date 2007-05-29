using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using System.Collections;
using NHibernate;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

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

        #region Query helpers

        private void AddMainQueryParameters(List<QueryParameter> parameters)
        {
            parameters.Add(new QueryParameter("cpsSchedulingStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("cpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1)));
        }

        private void AddSubQueryParameters(List<QueryParameter> parameters)
        {
            parameters.Add(new QueryParameter("mpsState", "SC"));
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date));
            parameters.Add(new QueryParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(1)));
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

        #endregion

        #region IRegistrationWorklistBroker Members

        #region Worklist

        public IList<WorklistItem> GetScheduledWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "SC"));
            AddMainQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCheckInWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "IP"));
            AddMainQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetInProgressWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition, _hqlMPSStartedSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "IP"));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCompletedWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "CM"));
            AddMainQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCancelledWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "DC"));
            AddMainQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        #endregion

        #region Worklist Counts

        public int GetScheduledWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "SC"));
            AddMainQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCheckInWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "IP"));
            AddMainQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetInProgressWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition, _hqlMPSStartedSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "IP"));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCompletedWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "CM"));
            AddMainQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCancelledWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "DC"));
            AddMainQueryParameters(parameters);

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
            parameters.Add(new QueryParameter("cpsState", "SC"));
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
            parameters.Add(new QueryParameter("cpsState", "SC"));
            parameters.Add(new QueryParameter("patient", patient));
            AddMainQueryParameters(parameters);

            IList list = DoQuery(hqlQuery, parameters);
            return (int)list[0];
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
            parameters.Add(new QueryParameter("cpsState1", "SC"));
            parameters.Add(new QueryParameter("cpsState2", "IP"));
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
            parameters.Add(new QueryParameter("cpsState1", "SC"));
            parameters.Add(new QueryParameter("cpsState2", "IP"));
            parameters.Add(new QueryParameter("patient", patient));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            IList list = DoQuery(hqlQuery, parameters);
            return (int)list[0];
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

        #endregion
    }
}
