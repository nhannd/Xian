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

        private const string _hqlSelectWorklist =       "select distinct pp from ModalityProcedureStep mps";
        private const string _hqlSelectCount =          "select count(distinct pp) from ModalityProcedureStep mps";
        private const string _hqlJoin =                 " join mps.RequestedProcedure rp" +
                                                        " join rp.Order o" +
                                                        " join o.Patient p" +
                                                        " join p.Profiles pp";
        private const string _hqlMainCondition =        " where mps.State = :mpsState" +
                                                        " and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd";
        private const string _hqlScheduledSubCondition =" and rp not in" +
                                                        " (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp where" +
                                                        " (cps.State = :cpsState and cps.StartTime between :cpsStartTimeBegin and :cpsStartTimeEnd))";
        private const string _hqlCheckInSubCondition =  " and rp in" +
                                                        " (select rp from CheckInProcedureStep cps join cps.RequestedProcedure rp where" +
                                                        " (cps.State = :cpsState and cps.StartTime between :cpsStartTimeBegin and :cpsStartTimeEnd))";

        private void AddMainQueryParameters(List<QueryParameter> parameters)
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

        private IList<WorklistItem> GetWorklist(string hqlQuery, List<QueryParameter> parameters)
        {
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            foreach (QueryParameter param in parameters)
            {
                query.SetParameter(param.Name, param.Value);
            }

            List<WorklistItem> results = new List<WorklistItem>();
            foreach (object tuple in query.List())
            {
                WorklistItem item = (WorklistItem)Activator.CreateInstance(typeof(WorklistItem), tuple);
                results.Add(item);
            }

            return results;
        }

        private int GetWorklistCount(string hqlQuery, List<QueryParameter> parameters)
        {
            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            foreach (QueryParameter param in parameters)
            {
                query.SetParameter(param.Name, param.Value);
            }

            IList list = query.List();
            return (int)list[0];
        }

        #region IRegistrationWorklistBroker Members

        public IList<WorklistItem> GetScheduledWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition, _hqlScheduledSubCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "SC"));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCheckInWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition, _hqlCheckInSubCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "SC"));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetInProgressWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "IP"));
            AddMainQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCompletedWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "CM"));
            AddMainQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCancelledWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectWorklist, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "DC"));
            AddMainQueryParameters(parameters);

            return GetWorklist(hqlQuery, parameters);
        }


        public int GetScheduledWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition, _hqlScheduledSubCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "SC"));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCheckInWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition, _hqlCheckInSubCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "SC"));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetInProgressWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "IP"));
            AddMainQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCompletedWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "CM"));
            AddMainQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCancelledWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectCount, _hqlJoin, _hqlMainCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "DC"));
            AddMainQueryParameters(parameters);

            return GetWorklistCount(hqlQuery, parameters);
        }

        public IList<RequestedProcedure> GetRequestedProcedureForCheckIn(Patient patient)
        {
            List<RequestedProcedure> results = new List<RequestedProcedure>();
            string hqlQuery = "select distinct rp from ModalityProcedureStep mps" +
                        " join mps.RequestedProcedure rp" +
                        " join rp.Order o" +
                        " join o.DiagnosticService ds" +
                        " join o.Patient p" +
                        _hqlMainCondition +
                        " and p = :patient" +
                        _hqlScheduledSubCondition;

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("mpsState", "SC"));
            parameters.Add(new QueryParameter("patient", patient));
            AddMainQueryParameters(parameters);
            AddSubQueryParameters(parameters);

            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            foreach (QueryParameter param in parameters)
            {
                query.SetParameter(param.Name, param.Value);
            }

            IList list = query.List();
            foreach (object tuple in list)
            {
                RequestedProcedure item = (RequestedProcedure)tuple;
                results.Add(item);
            }

            return results;
        }

        public IList<RequestedProcedure> GetRequestedProcedureForPatientPreview(Patient patient)
        {
            List<RequestedProcedure> results = new List<RequestedProcedure>();
            string hqlQuery = "select distinct rp from ModalityProcedureStep mps" +
                        " join mps.RequestedProcedure rp" +
                        " join rp.Order o" +
                        " join o.Patient p" +
                        " where p = :patient" +
                        " and mps.Scheduling.StartTime between :mpsSchedulingStartTimeBegin and :mpsSchedulingStartTimeEnd";

            IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
            query.SetParameter("patient", patient);
            query.SetParameter("mpsSchedulingStartTimeBegin", Platform.Time.Date.AddDays(-14));
            query.SetParameter("mpsSchedulingStartTimeEnd", Platform.Time.Date.AddDays(14));

            IList list = query.List();
            foreach (object tuple in list)
            {
                RequestedProcedure item = (RequestedProcedure)tuple;
                results.Add(item);
            }

            return results;
        }

        #endregion
    }
}
