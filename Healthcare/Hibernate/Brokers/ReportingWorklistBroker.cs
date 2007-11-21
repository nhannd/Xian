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
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Workflow;
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

        private const string _hqlSelectCount = "select count(*)";
        private const string _hqlSelectWorklist =
            "select rps, pp, o.AccessionNumber, o.Priority, rpt.Name, ds.Name, rps.State, v.PatientClass";

        private const string _hqlFromReportingStep = " from ReportingProcedureStep rps";
        private const string _hqlFromInterpretationStep = " from InterpretationStep rps";
        private const string _hqlFromTranscriptionStep = " from TranscriptionStep rps";
        private const string _hqlFromVerificationStep = " from VerificationStep rps";
        private const string _hqlFromPublicationStep = " from PublicationStep rps";
        private const string _hqlFromProtocolStep = " from ProtocolProcedureStep rps";

        private const string _hqlToBeReportedWorklist = _hqlSelectWorklist + _hqlFromInterpretationStep;
        private const string _hqlToBeReportedCount = _hqlSelectCount + _hqlFromInterpretationStep;

        private const string _hqlProtocollingWorklist = _hqlSelectWorklist + _hqlFromProtocolStep;
        private const string _hqlProtocollingCount = _hqlSelectCount + _hqlFromProtocolStep;

        private const string _hqlSelectTranscriptionWorklist = _hqlSelectWorklist + _hqlFromTranscriptionStep;
        private const string _hqlSelectTranscriptionCount = _hqlSelectCount + _hqlFromTranscriptionStep;

        private const string _hqlSelectVerificationWorklist = _hqlSelectWorklist + _hqlFromVerificationStep;
        private const string _hqlSelectVerificationCount = _hqlSelectCount + _hqlFromVerificationStep;

        private const string _hqlSelectPublicationWorklist = _hqlSelectWorklist + _hqlFromPublicationStep;
        private const string _hqlSelectPublicationCount = _hqlSelectCount + _hqlFromPublicationStep;

        private const string _hqlJoin =
            " join rps.RequestedProcedure rp" +
            " join rp.Type rpt" +
            " join rp.Order o" +
            " join o.DiagnosticService ds" +
            " join o.Visit v" +
            " join o.Patient p" +
            " join p.Profiles pp";

        private const string _hqlJoinReportPart =
            " join rps.ReportPart rpp";

        private const string _hqlJoinProtocol =
            " join rps.Protocol protocol";

        private const string _hqlSingleStateCondition =
            " where rps.State = :rpsState";

        private const string _hqlDualStateCondition =
            " where (rps.State = :rpsState or rps.State = :rpsState2)";

        private const string _hqlProtocolStatusCondition =
            " and protocol.Status = :protocolStatus";

        private const string _hqlCommunualWorklistCondition = 
            " and rps.Scheduling.Performer is NULL";

        private const string _hqlScheduledPerformerCondition = 
            " and rps.Scheduling.Performer = :scheduledPerformingStaff";

        private const string _hqlPerformerCondition =
            " and rps.Performer = :performingStaff";

        private const string _hqlWorklistSubQuery = 
            " and rp.Type in" +
            " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = :worklist)";

        private const string _hqlReportPartInterpretorCondition =
            " and rpp.Interpretor = :interpretorStaff";

        private const string _hqlReportPartVerifierCondition =
            " and rpp.Verifier = :verifierStaff";

        private const string _hqlSupervisorSubQuery =
            " and rpp.Supervisor = :supervisorStaff";

        private const string _hqlNoSupervisorSubQuery =
            " and rpp.Supervisor is NULL";

        #region Query helpers

        private IList<WorklistItem> GetWorklist(string hqlQuery, IEnumerable<QueryParameter> parameters)
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

        private int GetWorklistCount(string hqlQuery, IEnumerable<QueryParameter> parameters)
        {
            IList list = DoQuery(hqlQuery, parameters);
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

        #region IReportingWorklistBroker Members

        #region Worklist

        public IList<WorklistItem> GetToBeReportedWorklist()
        {
            return GetToBeReportedWorklist(null);
        }

        public IList<WorklistItem> GetToBeReportedWorklist(ReportingToBeReportedWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlToBeReportedWorklist, 
                _hqlJoin, 
                _hqlSingleStateCondition, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));

            if(worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetDraftWorklist(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlToBeReportedWorklist, 
                _hqlJoin, 
                _hqlDualStateCondition, _hqlScheduledPerformerCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("scheduledPerformingStaff", currentStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetInTranscriptionWorklist(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectTranscriptionWorklist, 
                _hqlJoin, 
                _hqlDualStateCondition, _hqlScheduledPerformerCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("scheduledPerformingStaff", currentStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetToBeVerifiedWorklist(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, 
                _hqlJoin, _hqlJoinReportPart, 
                _hqlDualStateCondition, _hqlScheduledPerformerCondition, _hqlReportPartInterpretorCondition, _hqlNoSupervisorSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("scheduledPerformingStaff", currentStaff));
            parameters.Add(new QueryParameter("interpretorStaff", currentStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetVerifiedWorklist(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectPublicationWorklist,
                _hqlJoin, _hqlJoinReportPart,
                _hqlDualStateCondition, _hqlReportPartVerifierCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.CM.ToString()));
            parameters.Add(new QueryParameter("verifierStaff", currentStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetResidentToBeVerifiedWorklist(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, 
                _hqlJoin, _hqlJoinReportPart, 
                _hqlDualStateCondition, _hqlReportPartInterpretorCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("interpretorStaff", currentStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetResidentVerifiedWorklist(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, 
                _hqlJoin, _hqlJoinReportPart, 
                _hqlSingleStateCondition, _hqlReportPartInterpretorCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.CM.ToString()));
            parameters.Add(new QueryParameter("interpretorStaff", currentStaff));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetMyResidentToBeVerifiedWorklist(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationWorklist, 
                _hqlJoin, _hqlJoinReportPart, 
                _hqlDualStateCondition, _hqlSupervisorSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("supervisorStaff", currentStaff));

            return GetWorklist(hqlQuery, parameters);
        }


        public IList<WorklistItem> GetToBeProtocolledWorklist()
        {
            return GetToBeProtocolledWorklist(null);
        }

        public IList<WorklistItem> GetToBeProtocolledWorklist(ReportingToBeProtocolledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlProtocollingWorklist,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlDualStateCondition, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));

            if (worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetToBeApprovedWorklist(Staff perfomingStaff)
        {
            string hqlQuery = String.Concat(_hqlProtocollingWorklist,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlSingleStateCondition, _hqlProtocolStatusCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("protocolStatus", ProtocolStatus.AA.ToString()));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetCompletedProtocolWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlProtocollingWorklist,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.CM.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.DC.ToString()));

            return GetWorklist(hqlQuery, parameters);
        }

        public IList<WorklistItem> GetSuspendedProtocolWorklist(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlProtocollingWorklist,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlSingleStateCondition, _hqlProtocolStatusCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SU.ToString()));
            parameters.Add(new QueryParameter("protocolStatus", ProtocolStatus.SU.ToString()));

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
            string hqlQuery = String.Concat(_hqlToBeReportedCount, 
                _hqlJoin, 
                _hqlSingleStateCondition, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));

            if (worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetDraftWorklistCount(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlToBeReportedCount, 
                _hqlJoin, 
                _hqlDualStateCondition, _hqlScheduledPerformerCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("scheduledPerformingStaff", currentStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetInTranscriptionWorklistCount(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectTranscriptionCount, 
                _hqlJoin, 
                _hqlDualStateCondition, _hqlScheduledPerformerCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("scheduledPerformingStaff", currentStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetToBeVerifiedWorklistCount(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, 
                _hqlJoin, _hqlJoinReportPart,
                _hqlDualStateCondition, _hqlScheduledPerformerCondition, _hqlReportPartInterpretorCondition, _hqlNoSupervisorSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("scheduledPerformingStaff", currentStaff));
            parameters.Add(new QueryParameter("interpretorStaff", currentStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetVerifiedWorklistCount(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectPublicationCount,
                _hqlJoin, _hqlJoinReportPart,
                _hqlDualStateCondition, _hqlReportPartVerifierCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.CM.ToString()));
            parameters.Add(new QueryParameter("verifierStaff", currentStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetResidentToBeVerifiedWorklistCount(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, 
                _hqlJoin, _hqlJoinReportPart, 
                _hqlDualStateCondition, _hqlReportPartInterpretorCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("interpretorStaff", currentStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetResidentVerifiedWorklistCount(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, 
                _hqlJoin, _hqlJoinReportPart, 
                _hqlSingleStateCondition, _hqlReportPartInterpretorCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.CM.ToString()));
            parameters.Add(new QueryParameter("interpretorStaff", currentStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetMyResidentToBeVerifiedWorklistCount(Staff currentStaff)
        {
            string hqlQuery = String.Concat(_hqlSelectVerificationCount, 
                _hqlJoin, _hqlJoinReportPart, 
                _hqlDualStateCondition, _hqlSupervisorSubQuery);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("supervisorStaff", currentStaff));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetToBeProtocolledWorklistCount()
        {
            return GetToBeProtocolledWorklistCount(null);
        }

        public int GetToBeProtocolledWorklistCount(ReportingToBeProtocolledWorklist worklist)
        {
            string hqlQuery = String.Concat(_hqlProtocollingCount,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlDualStateCondition, _hqlCommunualWorklistCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SC.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.IP.ToString()));

            if (worklist != null)
            {
                hqlQuery += _hqlWorklistSubQuery;
                parameters.Add(new QueryParameter("worklist", worklist));
            }

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetToBeApprovedCount(Staff perfomingStaff)
        {
            string hqlQuery = String.Concat(_hqlProtocollingCount,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlSingleStateCondition, _hqlProtocolStatusCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.IP.ToString()));
            parameters.Add(new QueryParameter("protocolStatus", ProtocolStatus.AA.ToString()));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetCompletedProtocolCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlProtocollingCount,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlDualStateCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.CM.ToString()));
            parameters.Add(new QueryParameter("rpsState2", ActivityStatus.DC.ToString()));

            return GetWorklistCount(hqlQuery, parameters);
        }

        public int GetSuspendedProtocolCount(Staff performingStaff)
        {
            string hqlQuery = String.Concat(_hqlProtocollingCount,
                                            _hqlJoin, _hqlJoinProtocol,
                                            _hqlSingleStateCondition, _hqlProtocolStatusCondition);

            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("rpsState", ActivityStatus.SU.ToString()));
            parameters.Add(new QueryParameter("protocolStatus", ProtocolStatus.SU.ToString()));

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
                              " where p = :patient" +
                              " and (rep.Status = :reportStatus1 or rep.Status = :reportStatus2)";

            List<Report> results = new List<Report>();
            List<QueryParameter> parameters = new List<QueryParameter>();
            parameters.Add(new QueryParameter("patient", patient));
            parameters.Add(new QueryParameter("reportStatus1", ReportStatus.F.ToString()));
            parameters.Add(new QueryParameter("reportStatus2", ReportStatus.C.ToString()));

            IList list = DoQuery(hqlQuery, parameters);
            foreach (object tuple in list)
            {
                Report item = (Report)tuple;
                results.Add(item);
            }

            return results;
        }


        public IList<WorklistItem> Search(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            StringBuilder hqlQuery = new StringBuilder();
            hqlQuery.Append(_hqlSelectWorklist);
            hqlQuery.Append(_hqlFromReportingStep);
            hqlQuery.Append(_hqlJoin);

            HqlQuery query = new HqlQuery(hqlQuery.ToString());
            query.Page = page;

            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("(rps.State != ? and rps.State != ?)", ActivityStatus.CM, ActivityStatus.DC));
            }
            else
            {
                // Active Set of RPS union with inactive set of verification Step
                query.Conditions.Add(new HqlCondition(" ((rps.State != ? and rps.State != ?) or (rps.class = VerificationStep and (rps.State = ? or rps.State = ?)))",
                    ActivityStatus.CM, ActivityStatus.DC, ActivityStatus.CM, ActivityStatus.DC));
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
            hqlQuery.Append(_hqlFromReportingStep);
            hqlQuery.Append(_hqlJoin);

            HqlQuery query = new HqlQuery(hqlQuery.ToString());

            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("rps.State in (?, ?)", ActivityStatus.SC, ActivityStatus.IP));
            }
            else
            {
                // Active Set of RPS union with inactive set of verification Step
                query.Conditions.Add(new HqlCondition("(rps.State in (?, ?) or (rps.class = VerificationStep and rps.State in (?, ?)))",
                    ActivityStatus.SC, ActivityStatus.IP, ActivityStatus.SC, ActivityStatus.IP));
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
