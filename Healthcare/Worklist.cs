#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using Iesi.Collections;
using Iesi.Collections.Generic;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    public abstract class Worklist : Entity, IWorklist
    {
        private string _name;
        private string _description;
        private readonly ISet<string> _users;
        private WorklistProcedureTypeGroupFilter _procedureTypeGroupFilter;
        private WorklistFacilityFilter _facilityFilter;
        private WorklistPatientClassFilter _patientClassFilter;
        private WorklistOrderPriorityFilter _orderPriorityFilter;
        private WorklistPortableFilter _portableFilter;
        private WorklistTimeFilter _timeFilter;

        public Worklist()
        {
            _users = new HashedSet<string>();

            _procedureTypeGroupFilter = new WorklistProcedureTypeGroupFilter();
            _facilityFilter = new WorklistFacilityFilter();
            _patientClassFilter = new WorklistPatientClassFilter();
            _orderPriorityFilter = new WorklistOrderPriorityFilter();
            _portableFilter = new WorklistPortableFilter();
            _timeFilter = new WorklistTimeFilter();
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string DisplayName
        {
            get { return _name; }
        }

        public virtual string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public virtual ISet<string> Users
        {
            get { return _users; }
        }

        #region Abstract members

        public abstract IList GetWorklistItems(IWorklistQueryContext wqc);
        public abstract int GetWorklistItemCount(IWorklistQueryContext wqc);
        public abstract WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc);
        public abstract Type ProcedureStepType { get; }

        #endregion

        #region Filters

        public WorklistProcedureTypeGroupFilter ProcedureTypeGroupFilter
        {
            get { return _procedureTypeGroupFilter; }
            set { _procedureTypeGroupFilter = value; }
        }

        public WorklistFacilityFilter FacilityFilter
        {
            get { return _facilityFilter; }
            set { _facilityFilter = value; }
        }

        public WorklistPatientClassFilter PatientClassFilter
        {
            get { return _patientClassFilter; }
            set { _patientClassFilter = value; }
        }

        public WorklistOrderPriorityFilter OrderPriorityFilter
        {
            get { return _orderPriorityFilter; }
            set { _orderPriorityFilter = value; }
        }

        public WorklistPortableFilter PortableFilter
        {
            get { return _portableFilter; }
            set { _portableFilter = value; }
        }

        public WorklistTimeFilter TimeFilter
        {
            get { return _timeFilter; }
            set { _timeFilter = value; }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Applies the time-range for this worklist to the specified search-condition.
        /// </summary>
        /// <remarks>
        /// If the <see cref="TimeFilter"/> is enabled, the range specified by the filter is applied.
        /// If the filter is not enabled, then the specified <paramref name="defaultValue"/> is applied.
        /// The <paramref name="defaultValue"/> may be null, in which case no time range is applied by default.
        /// </remarks>
        /// <param name="condition"></param>
        /// <param name="defaultValue"></param>
        protected void ApplyTimeRange(ISearchCondition<DateTime> condition, WorklistTimeRange defaultValue)
        {
            WorklistTimeRange range = _timeFilter.IsEnabled ? _timeFilter.Value : defaultValue;
            if(range != null)
                range.Apply(condition, Platform.Time);
        }

        /// <summary>
        /// Applies the time-range for this worklist to the specified search-condition.
        /// </summary>
        /// <remarks>
        /// If the <see cref="TimeFilter"/> is enabled, the range specified by the filter is applied.
        /// If the filter is not enabled, then the specified <paramref name="defaultValue"/> is applied.
        /// The <paramref name="defaultValue"/> may be null, in which case no time range is applied by default.
        /// </remarks>
        /// <param name="condition"></param>
        /// <param name="defaultValue"></param>
        protected void ApplyTimeRange(ISearchCondition<DateTime?> condition, WorklistTimeRange defaultValue)
        {
            WorklistTimeRange range = _timeFilter.IsEnabled ? _timeFilter.Value : defaultValue;
            if (range != null)
                range.Apply(condition, Platform.Time);
        }

        #endregion
    }
}