using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
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

        private const string _hqlSelectInterpretationWorklist = 
            "select rps, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority, rpt.Name, ds.Name, rps.State from InterpretationStep rps";
        private const string _hqlSelectInterpretationCount = 
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
            " where rps.State = :rpsState and rps.Performer is NULL";

        private const string _hqlMySingleStateCondition =
            " where rps.State = :rpsState and rps.Performer = :performingStaff";

        private const string _hqlMyDualStateCondition =
            " where (rps.State = :rpsState or rps.State = :rpsState2)" +
            " and rps.Performer = :performingStaff";


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
            try
            {
                IList list = DoQuery(hqlQuery, parameters);
                return (int)list[0];
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        private IList DoQuery(string hqlQuery, List<QueryParameter> parameters)
        {
            try
            {
                IQuery query = this.Context.CreateHibernateQuery(hqlQuery);
                foreach (QueryParameter param in parameters)
                {
                    query.SetParameter(param.Name, param.Value);
                }

                return query.List();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region IReportingWorklistBroker Members

        #region Worklist

        public IList<WorklistItem> GetScheduledInterpretationWorklist()
        {
            string hqlQuery = String.Concat(_hqlSelectInterpretationWorklist, _hqlJoin, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetMyInterpretationWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectInterpretationWorklist, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetMyTranscriptionWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectTranscriptionWorklist, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetMyVerificationWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetMyVerifiedWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, _hqlJoin, _hqlMySingleStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        #endregion

        #region Worklist Count

        public int GetScheduledInterpretationWorklistCount()
        {
            string hqlQuery = String.Concat(_hqlSelectInterpretationCount, _hqlJoin, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetMyInterpretationWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectInterpretationCount, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetMyTranscriptionWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectTranscriptionCount, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetMyVerificationWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, _hqlJoin, _hqlMyDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetMyVerifiedWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, _hqlJoin, _hqlMySingleStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "CM"));
            parameters.Add(new QueryParameter("performingStaff", performingStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        #endregion

        #endregion
    }
}
