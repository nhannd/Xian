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

        private const string _hqlSingleStateCondition =
            " where rps.State = :rpsState";

        private const string _hqlCommunualWorklistCondition = 
            _hqlSingleStateCondition +
            " and rps.Scheduling.Performer is NULL";

        private const string _hqlMySingleStateCondition =
            _hqlSingleStateCondition +
            " and rps.Scheduling.Performer = :performingStaff";

        private const string _hqlDualStateCondition =
            " where (rps.State = :rpsState or rps.State = :rpsState2)";

        private const string _hqlMyDualStateCondition = 
            _hqlDualStateCondition + 
            " and rps.Scheduling.Performer = :performingStaff";

        private const string _hqlWorklistSubQuery = 
            " and rp.Type in" +
            " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = :worklist)";

        private const string _hqlSupervisorSubQuery = 
            " and rp in" +
            " (select report.Procedure from Report report where report.Supervisor = :supervisorStaff)";

        #region Query helpers

        private IList<WorklistItem> GetWorklist(string hqlQuery, IEnumerable<QueryParameter> parameters)
        {
            try
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
            catch (Exception e)
            {
                throw e;
            }
        }

        private int GetWorklistCount(string hqlQuery, IEnumerable<QueryParameter> parameters)
        {
            try
            {
                IList list = DoQuery(hqlQuery, parameters);
                return (int)list[0];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private IList DoQuery(string hqlQuery, IEnumerable<QueryParameter> parameters)
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

        public IList<WorklistItem> GetResidentToBeVerifiedWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, _hqlJoin, _hqlDualStateCondition, _hqlSupervisorSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("supervisorStaff", performingStaff));

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

        public int GetResidentToBeVerifiedWorklistCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, _hqlJoin, _hqlDualStateCondition, _hqlSupervisorSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", "SC"));
            parameters.Add(new QueryParameter("rpsState2", "IP"));
            parameters.Add(new QueryParameter("supervisorStaff", performingStaff));

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

        public IList<WorklistItem> Search(
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

            hqlQuery.Append("select rps, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority, rpt.Name, ds.Name, rps.State");

            hqlQuery.Append(" from ReportingProcedureStep rps");
            hqlQuery.Append(" join rps.RequestedProcedure rp" +
                            " join rp.Type rpt" +
                            " join rp.Order o" +
                            " join o.DiagnosticService ds" +
                            " join o.Patient p" +
                            " join p.Profiles pp");

            if (showActiveOnly)
            {
                hqlQuery.Append(" where (rps.State != :rpsState1 and rps.State != :rpsState2)");
                parameters.Add(new QueryParameter("rpsState1", "CM"));
                parameters.Add(new QueryParameter("rpsState2", "DC"));
            }
            else
            {
                // Active Set of RPS union with inactive set of verification Step
                hqlQuery.Append(" where (");
                hqlQuery.Append("(rps.State != :rpsState1 and rps.State != :rpsState2)");
                hqlQuery.Append(" or ");
                hqlQuery.Append("(rps.class = VerificationStep and (rps.State = :rpsState1 or rps.State = :rpsState2))");
                hqlQuery.Append(")");
                parameters.Add(new QueryParameter("rpsState1", "CM"));
                parameters.Add(new QueryParameter("rpsState2", "DC"));
            }

            if (!String.IsNullOrEmpty(accessionNumber))
            {
                hqlQuery.Append(" and o.AccessionNumber = :accessionNumber");
                parameters.Add(new QueryParameter("accessionNumber", accessionNumber));
            }

            if (!String.IsNullOrEmpty(mrnID))
            {
                hqlQuery.Append(" and pp.Mrn.Id = :mrnID");
                parameters.Add(new QueryParameter("mrnID", mrnID));
            }

            if (!String.IsNullOrEmpty(mrnAssigningAuthority))
            {
                hqlQuery.Append(" and pp.Mrn.AssigningAuthority = :mrnAssigningAuthority");
                parameters.Add(new QueryParameter("mrnAssigningAuthority", mrnAssigningAuthority));
            }

            if (!String.IsNullOrEmpty(healthcardID))
            {
                hqlQuery.Append(" and pp.Healthcard.Id = :healthcardID");
                parameters.Add(new QueryParameter("healthcardID", healthcardID));
            }

            if (!String.IsNullOrEmpty(familyName))
            {
                hqlQuery.Append(" and pp.Name.FamilyName like :familyName");
                parameters.Add(new QueryParameter("familyName", familyName + "%"));
            }

            if (!String.IsNullOrEmpty(givenName))
            {
                hqlQuery.Append(" and pp.Name.GivenName like :givenName");
                parameters.Add(new QueryParameter("givenName", givenName + "%"));
            }

            return GetWorklist(hqlQuery.ToString(), parameters);
        }

        #endregion
    }
}
