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
using ClearCanvas.Healthcare.Workflow.Registration;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class RegistrationWorklistBroker : Broker, IRegistrationWorklistBroker
    {
        private const string _hqlSelectWorklist =       "select distinct o from RequestedProcedure rp";
        private const string _hqlSelectCount =          "select count(distinct o) from RequestedProcedure rp";
        private const string _hqlJoin =                 " join rp.Order o";

        private const string _hqlSelectProtocolWorklist     = "select distinct o from ProtocolProcedureStep ps";
        private const string _hqlSelectProtocolCount        = "select count(distinct o) from ProtocolProcedureStep ps";
        private const string _hqlProtocolJoin               = " join ps.RequestedProcedure rp" +
                                                              " join rp.Order o";

        private const string _hqlSearchWorklist             = "select o from Order o";
        private const string _hqlSearchCount                = "select count(o) from Order o";

        // Share constants
        private const string _hqlJoinProfile                = " join o.Patient p join p.Profiles pp";
        private const string _hqlWorklistSubQuery           = "rp.Type in (select distinct rpt from Worklist w" +
                                                              " join w.RequestedProcedureTypeGroups rptg join rptg.RequestedProcedureTypes rpt where w = ?)";

        #region IRegistrationWorklistBroker members

        public IList<WorklistItem> GetWorklist(RegistrationWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectWorklist, _hqlJoin, _hqlJoinProfile));
            ConstructWorklistCondition(query, where, worklist);
            return DoQuery(query);
        }

        public int GetWorklistCount(RegistrationWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectCount, _hqlJoin, _hqlJoinProfile));
            ConstructWorklistCondition(query, where, worklist);
            return DoQueryCount(query);
        }

        public IList<WorklistItem> GetProtocolWorklist(RegistrationWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectProtocolWorklist, _hqlProtocolJoin));
            ConstructWorklistCondition(query, where, worklist);
            return DoQuery(query);
        }

        public int GetProtocolWorklistCount(RegistrationWorklistItemSearchCriteria[] where, Worklist worklist)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSelectProtocolCount, _hqlProtocolJoin));
            ConstructWorklistCondition(query, where, worklist);
            return DoQueryCount(query);
        }

        public IList<WorklistItem> Search(WorklistItemSearchCriteria[] where, SearchResultPage page, bool showActiveOnly)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSearchWorklist, _hqlJoinProfile));
            query.Page = page;
            ConstructSearchCondition(query, where, showActiveOnly);
            return DoQuery(query);
        }

        public int SearchCount(WorklistItemSearchCriteria[] where, bool showActiveOnly)
        {
            HqlQuery query = new HqlQuery(string.Concat(_hqlSearchCount, _hqlJoinProfile));
            ConstructSearchCondition(query, where, showActiveOnly);
            return DoQueryCount(query);
        }

        #endregion

        #region Private Helpers

        private static void ConstructWorklistCondition(HqlQuery query, IEnumerable<RegistrationWorklistItemSearchCriteria> where, Worklist worklist)
        {
            HqlOr or = new HqlOr();
            foreach (RegistrationWorklistItemSearchCriteria c in where)
            {
                HqlAnd and = new HqlAnd();

                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", c.Order));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("rp", c.RequestedProcedure));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("rp.ProcedureCheckIn", c.ProcedureCheckIn));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("ps", c.ProtocolProcedureStep));
                and.Conditions.AddRange(HqlCondition.FromSearchCriteria("pp", c.PatientProfile));

                if (and.Conditions.Count > 0)
                    or.Conditions.Add(and);

                query.Sorts.AddRange(HqlSort.FromSearchCriteria("o", c.Order));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("rp", c.RequestedProcedure));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("rp.ProcedureCheckIn", c.ProcedureCheckIn));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("ps", c.ProtocolProcedureStep));
                query.Sorts.AddRange(HqlSort.FromSearchCriteria("pp", c.PatientProfile));
            }

            if (or.Conditions.Count > 0)
                query.Conditions.Add(or);

            if (worklist != null)
            {
                query.Conditions.Add(new HqlCondition(_hqlWorklistSubQuery, worklist));
            }
        }

        private static void ConstructSearchCondition(HqlQuery query, IEnumerable<WorklistItemSearchCriteria> where, bool showActiveOnly)
        {
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
        }

        private List<WorklistItem> DoQuery(HqlQuery query)
        {
            IList<object> list = ExecuteHql<object>(query);
            List<WorklistItem> results = new List<WorklistItem>();
            foreach (object tuple in list)
            {
                WorklistItem item = (WorklistItem) Activator.CreateInstance(typeof (WorklistItem), tuple);
                results.Add(item);
            }

            return results;
        }

        private int DoQueryCount(HqlQuery query)
        {
            return (int)ExecuteHqlUnique<long>(query);
        }

        #endregion
    }
}
