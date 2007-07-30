using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using NHibernate;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ReportingWorklistBroker : Broker, IReportingWorklistBroker
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

        private const string _hqlToBeReportedWorklist = 
            "select rps, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority, rpt.Name, ds.Name, rps.State from InterpretationStep rps";
        private const string _hqlToBeReportedCount = 
            "select count(*) from InterpretationStep rps";

        private const string _hqlSelectTranscriptionWorklist =
            "select rps, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority, rpt.Name, ds.Name, rps.State from TranscriptionStep rps";
        private const string _hqlSelectTranscriptionCount =
            "select count(*) from TranscriptionStep rps";

        private const string _hqlSelectVerificationWorklist =
            "select rps, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority, rpt.Name, ds.Name, rps.State from VerificationStep rps";
        private const string _hqlSelectVerificationCount =
            "select count(*) from VerificationStep rps";

        private const string _hqlJoin =
            " join rps.RequestedProcedure rp" +
            " join rp.Type rpt" +
            " join rp.Order o" +
            " join o.DiagnosticService ds" +
            " join o.Patient p" +
            " join p.Profiles pp";

        private const string _hqlCommunualWorklistCondition =
            " where rps.State = :rpsState and rps.Scheduling.Performer is NULL";

        private const string _hqlMySingleStateCondition =
            " where rps.State = :rpsState and rps.Scheduling.Performer = :performingStaff";

        private const string _hqlMyDualStateCondition =
            " where (rps.State = :rpsState or rps.State = :rpsState2)" +
            " and rps.Scheduling.Performer = :performingStaff";

        private const string _hqlWorklistSubQuery = 
            " and rp.Type in" +
            " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = :worklist)";


        #region Query helpers

        private IList<WorklistItem> GetWorklist(string hqlQuery, List<QueryParameter> parameters)
        {
            List<WorklistItem> results = new List<WorklistItem>();

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object[] tuple in list)
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

        #region IReportingWorklistBroker Members

        #region Worklist

        public IList<WorklistItem> GetToBeReportedWorklist()
        {
            return GetToBeReportedWorklist(null);
        }

        public IList<WorklistItem> GetToBeReportedWorklist(ReportingToBeReportedWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlToBeReportedWorklist, _hqlJoin, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));

            if(worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetDraftWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlToBeReportedWorklist, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetInTranscriptionWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectTranscriptionWorklist, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetToBeVerifiedWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetVerifiedWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, _hqlJoin, _hqlMySingleStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "CM"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        #endregion

        #region Worklist Count

        public int GetToBeReportedWorklistCount()
        {
            return GetToBeReportedWorklistCount(null);
        }

        public int GetToBeReportedWorklistCount(ReportingToBeReportedWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlToBeReportedCount, _hqlJoin, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));

            if (worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetDraftWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlToBeReportedCount, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetInTranscriptionWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectTranscriptionCount, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetToBeVerifiedWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetVerifiedWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, _hqlJoin, _hqlMySingleStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "CM"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        #endregion

        public IList<Report> GetPriorReport(Patient patient)
        {
            string hqlQuery = "select rep from Report rep" +
                " join rep.Procedure rp" +
                " join rp.Type rpt" +
                " join rp.Order o" +
                " join o.DiagnosticService ds" +
                " join o.Patient p" +
                " where p = :patient";

            List<Report> results = new List<Report>();
            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("patient", patient));

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object tuple in list)
            {
                Report item = (Report)tuple;
                results.Add(item);
            }

            return results;
        }

        #endregion
    }
}
