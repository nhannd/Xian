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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class ModalityWorklistBroker : WorklistItemBrokerBase<WorklistItem>, IModalityWorklistBroker
    {
        private const string _hqlSelectWorklist =
           "select ps, rp, o, p, pp, pp.Mrn, pp.Name, o.AccessionNumber, o.Priority," +
           " v.PatientClass, ds.Name, rpt.Name, ps.Scheduling.StartTime, m" +
           " from ModalityProcedureStep ps";

        private const string _hqlSelectCount = 
            "select count(*)" +
            " from ModalityProcedureStep ps";

        private const string _hqlJoin =
            " join ps.Type spst" +
            " join ps.Modality m" +
            " join ps.RequestedProcedure rp" +
            " join rp.Type rpt" +
            " join rp.Order o" +
            " join o.DiagnosticService ds" +
            " join o.Visit v" +
            " join o.Patient p" +
            " join p.Profiles pp";

        private const string _hqlUndocumentedSubCondition =
            "rp in" +
            " (select rp from DocumentationProcedureStep dps join dps.RequestedProcedure rp where" +
            " (dps.State = ? and dps.StartTime between ? and ?))";

        private const string _hqlWorklistSubQuery = 
            "rp.Type in" +
            " (select distinct rpt from Worklist w join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = ?)";

        #region IModalityWorklistBroker Members

        public IList<WorklistItem> GetWorklist(ModalityWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectWorklist, _hqlJoin));
            ConstructWorklistCondition(query, where, worklist, false);
            return DoQuery(query);
        }

        public int GetWorklistCount(ModalityWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectCount, _hqlJoin));
            ConstructWorklistCondition(query, where, worklist, false);
            return DoQueryCount(query);
        }

        public IList<WorklistItem> GetUndocumentedWorklist(ModalityWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectWorklist, _hqlJoin));
            ConstructWorklistCondition(query, where, worklist, true);
            return DoQuery(query);
        }

        public int GetUndocumentedWorklistCount(ModalityWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectCount, _hqlJoin));
            ConstructWorklistCondition(query, where, worklist, true);
            return DoQueryCount(query);
        }

        public IList<WorklistItem> Search(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectWorklist, _hqlJoin));
            query.Page = page;
            ConstructSearchCondition(query, where, showActiveOnly);
            return DoQuery(query);
        }

        public int SearchCount(WorklistItemSearchCriteria[] where, bool showActiveOnly)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectCount, _hqlJoin));
            ConstructSearchCondition(query, where, showActiveOnly);
            return DoQueryCount(query);
        }

        #endregion

        #region Private Helpers

        private static void ConstructWorklistCondition(HqlQuery query, IEnumerable<ModalityWorklistItemSearchCriteria> where, Worklist worklist, bool undocumentedCondition)
        {
            HqlOr or = new HqlOr();
            foreach (ModalityWorklistItemSearchCriteria c in where)
            {
                HqlAnd and = new HqlAnd();

                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", c.Order));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("pp", c.PatientProfile));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("rp", c.RequestedProcedure));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("rp.ProcedureCheckIn", c.ProcedureCheckIn));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("ps", c.ModalityProcedureStep));

                if (and.Conditions.Count > 0)
                    or.Conditions.Add(and);

                query.Sorts.AddRange(HqlSort.FromSearchCriteria("o", c.Order));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("pp", c.PatientProfile));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("rp", c.RequestedProcedure));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("rp.ProcedureCheckIn", c.ProcedureCheckIn));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("ps", c.ModalityProcedureStep));
            }

            if (or.Conditions.Count > 0)
                query.Conditions.Add(or);

            if (worklist != null && !worklist.RequestedProcedureTypeGroups.IsEmpty)
            {
                query.Conditions.Add(new HqlCondition(_hqlWorklistSubQuery, worklist));
            }

            if (undocumentedCondition)
            {
                query.Conditions.Add(new HqlCondition(_hqlUndocumentedSubCondition, ActivityStatus.IP, Platform.Time.Date, Platform.Time.Date.AddDays(1)));
            }
        }

        private static void ConstructSearchCondition(HqlQuery query, IEnumerable<WorklistItemSearchCriteria> where, bool showActiveOnly)
        {
            if (showActiveOnly)
            {
                query.Conditions.Add(new HqlCondition("ps.State in (?, ?)", ActivityStatus.SC, ActivityStatus.IP));
            }

            HqlOr or = new HqlOr();
            foreach (ModalityWorklistItemSearchCriteria c in where)
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
        }

        #endregion
    }
}
