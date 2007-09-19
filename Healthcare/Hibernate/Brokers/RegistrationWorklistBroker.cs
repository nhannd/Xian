using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
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
            return GetScheduledWorklist(null);
        }

        public IList<WorklistItem> GetScheduledWorklist(RegistrationScheduledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("cpsState", "SC"));
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
            parameters.Add(new QueryParameter("cpsState", "IP"));
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
            parameters.Add(new QueryParameter("cpsState", "IP"));
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
            parameters.Add(new QueryParameter("cpsState", "CM"));
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
            parameters.Add(new QueryParameter("cpsState", "DC"));
            AddMainQueryParameters(parameters);

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
            parameters.Add(new QueryParameter("cpsState", "SC"));
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
            parameters.Add(new QueryParameter("cpsState", "IP"));
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
            parameters.Add(new QueryParameter("cpsState", "IP"));
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
            parameters.Add(new QueryParameter("cpsState", "CM"));
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
            parameters.Add(new QueryParameter("cpsState", "DC"));
            AddMainQueryParameters(parameters);

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

        public IList<WorklistItem> Search(
            string mrnID,
            string mrnAssigningAuthority,
            string healthcardID,
            string familyName,
            string givenName,
            string accessionNumber,
            bool showActiveOnly)
        {
            IList<WorklistItem> searchResults =
                OrderSearch(mrnID, mrnAssigningAuthority, healthcardID, familyName, givenName, accessionNumber,
                            showActiveOnly);

            // Some patient may not have an order, search for patient profile
            if (searchResults.Count > 0)
                searchResults = PatientSearch(mrnID, mrnAssigningAuthority, healthcardID, familyName, givenName);

            return searchResults;
        }

        private IList<WorklistItem> OrderSearch(
            string mrnID,
            string mrnAssigningAuthority,
            string healthcardID,
            string familyName,
            string givenName,
            string accessionNumber,
            bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            List<QueryParameter> parameters = new List<QueryParameter>();

            hqlQuery.Append("select distinct o");
            hqlQuery.Append(" from Order o");
            hqlQuery.Append(" join o.Patient p" +
                            " join p.Profiles pp");

            string conditionPrefix = " where";
            if (showActiveOnly)
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" (o.Status = :orderStatus1 or o.Status = :orderStatus2)");
                parameters.Add(new QueryParameter("orderStatus1", "SC"));
                parameters.Add(new QueryParameter("orderStatus2", "IP"));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(accessionNumber))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" o.AccessionNumber = :accessionNumber");
                parameters.Add(new QueryParameter("accessionNumber", accessionNumber));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(mrnID))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Mrn.Id = :mrnID");
                parameters.Add(new QueryParameter("mrnID", mrnID));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(mrnAssigningAuthority))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Mrn.AssigningAuthority = :mrnAssigningAuthority");
                parameters.Add(new QueryParameter("mrnAssigningAuthority", mrnAssigningAuthority));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(healthcardID))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Healthcard.Id = :healthcardID");
                parameters.Add(new QueryParameter("healthcardID", healthcardID));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(familyName))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Name.FamilyName like :familyName");
                parameters.Add(new QueryParameter("familyName", familyName + "%"));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(givenName))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Name.GivenName like :givenName");
                parameters.Add(new QueryParameter("givenName", givenName + "%"));
                conditionPrefix = " and";
            }

            return GetWorklist(hqlQuery.ToString(), parameters);
        }

        private IList<WorklistItem> PatientSearch(
            string mrnID,
            string mrnAssigningAuthority,
            string healthcardID,
            string familyName,
            string givenName)
        {
            StringBuilder hqlQuery = new StringBuilder();
            List<QueryParameter> parameters = new List<QueryParameter>();

            hqlQuery.Append("select distinct pp");
            hqlQuery.Append(" from PatientProfile pp");

            string conditionPrefix = " where";
            if (!String.IsNullOrEmpty(mrnID))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Mrn.Id = :mrnID");
                parameters.Add(new QueryParameter("mrnID", mrnID));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(mrnAssigningAuthority))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Mrn.AssigningAuthority = :mrnAssigningAuthority");
                parameters.Add(new QueryParameter("mrnAssigningAuthority", mrnAssigningAuthority));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(healthcardID))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Healthcard.Id = :healthcardID");
                parameters.Add(new QueryParameter("healthcardID", healthcardID));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(familyName))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Name.FamilyName like :familyName");
                parameters.Add(new QueryParameter("familyName", familyName + "%"));
                conditionPrefix = " and";
            }

            if (!String.IsNullOrEmpty(givenName))
            {
                hqlQuery.Append(conditionPrefix);
                hqlQuery.Append(" pp.Name.GivenName like :givenName");
                parameters.Add(new QueryParameter("givenName", givenName + "%"));
                conditionPrefix = " and";
            }

            return GetWorklist(hqlQuery.ToString(), parameters);
        }

        #endregion
    }
}
