#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class WorklistBroker : EntityBroker<Worklist, WorklistSearchCriteria>, IWorklistBroker
    {

        #region IWorklistBroker Members

        public IList<Worklist> Find(Staff staff, IEnumerable<string> worklistClassNames)
        {
            HqlProjectionQuery query = GetBaseQuery();
            AddStaffConditions(query, staff);
            AddClassConditions(query, worklistClassNames);

            return ExecuteHql<Worklist>(query);
        }

        public IList<Worklist> Find(string name, bool includeUserDefinedWorklists, IEnumerable<string> worklistClassNames, SearchResultPage page)
        {
            HqlProjectionQuery query = GetBaseQuery();

            if (!string.IsNullOrEmpty(name))
                query.Conditions.Add(new HqlCondition("w.Name like ?", string.Format("%{0}%", name)));

            if (!includeUserDefinedWorklists)
                query.Conditions.Add(new HqlCondition("(w.Owner.Staff is null and w.Owner.Group is null)"));

            AddClassConditions(query, worklistClassNames);
            query.Page = page;

            return ExecuteHql<Worklist>(query);
        }

        public Worklist FindOne(string name, string worklistClassName)
        {
            HqlQuery query = new HqlQuery("from Worklist w");
            query.Conditions.Add(new HqlCondition("w.Name = ?", name));
            query.Conditions.Add(new HqlCondition("w.class = " + worklistClassName));

            IList<Worklist> worklists = ExecuteHql<Worklist>(query);
            if (worklists.Count == 0)
                throw new EntityNotFoundException(string.Format("Worklist {0}, class {1} not found.", name, worklistClassName), null);

            return CollectionUtils.FirstElement(worklists);
        }

        #endregion

        private HqlProjectionQuery GetBaseQuery()
        {
            HqlProjectionQuery query = new HqlProjectionQuery(new HqlFrom("Worklist", "w"));
            query.Selects.Add(new HqlSelect("w"));
            query.SelectDistinct = true;
            return query;
        }

        private void AddClassConditions(HqlProjectionQuery query, IEnumerable<string> worklistClassNames)
        {
            HqlOr classOr = new HqlOr();
            foreach (string className in worklistClassNames)
            {
                classOr.Conditions.Add(new HqlCondition("w.class = " + className));
            }
            query.Conditions.Add(classOr);
        }

        private void AddStaffConditions(HqlProjectionQuery query, Staff staff)
        {
            query.Froms.Add(new HqlFrom("Staff", "s"));
            query.Conditions.Add(new HqlCondition("s = ?", staff));

            HqlOr staffOr = new HqlOr();
            staffOr.Conditions.Add(new HqlCondition("s in elements(w.StaffSubscribers)"));
            staffOr.Conditions.Add(new HqlCondition("s in (select elements(sg.Members) from StaffGroup sg where sg in elements(w.GroupSubscribers))"));
            query.Conditions.Add(staffOr);
        }
    }
}
