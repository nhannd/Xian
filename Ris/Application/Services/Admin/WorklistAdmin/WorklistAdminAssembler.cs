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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.UserAdmin;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Application.Services.Admin.UserAdmin;
using System;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    internal class WorklistAdminAssembler
    {
        public WorklistAdminDetail GetWorklistDetail(Worklist worklist, IPersistenceContext context)
        {
            WorklistAdminDetail detail = new WorklistAdminDetail(worklist.GetRef(), worklist.Name, worklist.Description, WorklistFactory.Instance.GetWorklistType(worklist));

            if (worklist.ProcedureTypeGroupFilter.IsEnabled)
            {
                ProcedureTypeGroupAssembler assembler = new ProcedureTypeGroupAssembler();
                detail.ProcedureTypeGroups = CollectionUtils.Map<ProcedureTypeGroup, ProcedureTypeGroupSummary>(
                    worklist.ProcedureTypeGroupFilter.Values,
                    delegate(ProcedureTypeGroup rptGroup) { return assembler.GetProcedureTypeGroupSummary(rptGroup, context); });
            }

            if (worklist.FacilityFilter.IsEnabled)
            {
                FacilityAssembler facilityAssembler = new FacilityAssembler();
                detail.Facilities = CollectionUtils.Map<Facility, FacilitySummary>(
                    worklist.FacilityFilter.Values,
                    delegate(Facility f) { return facilityAssembler.CreateFacilitySummary(f); });
                detail.FilterByWorkingFacility = worklist.FacilityFilter.IncludeWorkingFacility;
            }
            if (worklist.PatientClassFilter.IsEnabled)
            {
                detail.PatientClasses = CollectionUtils.Map<PatientClassEnum, EnumValueInfo>(
                    worklist.PatientClassFilter.Values,
                    delegate(PatientClassEnum p) { return EnumUtils.GetEnumValueInfo(p); });
            }

            if (worklist.OrderPriorityFilter.IsEnabled)
            {
                detail.OrderPriorities = CollectionUtils.Map<OrderPriorityEnum, EnumValueInfo>(
                    worklist.OrderPriorityFilter.Values,
                    delegate(OrderPriorityEnum p) { return EnumUtils.GetEnumValueInfo(p); });
            }

            if(worklist.PortableFilter.IsEnabled)
            {
                detail.Portabilities = new List<bool>();
                detail.Portabilities.Add(worklist.PortableFilter.Value);
            }

            if (worklist.TimeFilter.IsEnabled && worklist.TimeFilter.Value != null)
            {
                if(worklist.TimeFilter.Value.Start != null)
                    detail.StartTime = CreateTimePointContract(worklist.TimeFilter.Value.Start);
                if(worklist.TimeFilter.Value.End != null)
                    detail.EndTime = CreateTimePointContract(worklist.TimeFilter.Value.End);
            }

            detail.Users = new List<string>(worklist.Users);

            return detail;
        }

        public WorklistAdminDetail.TimePoint CreateTimePointContract(WorklistTimePoint tp)
        {
            return tp.IsFixed ? new WorklistAdminDetail.TimePoint(tp.FixedValue, tp.Resolution) :
                new WorklistAdminDetail.TimePoint(tp.RelativeValue, tp.Resolution);
        }

        public WorklistAdminSummary GetWorklistSummary(Worklist worklist, IPersistenceContext context)
        {
            return new WorklistAdminSummary(worklist.GetRef(), worklist.Name, worklist.Description, WorklistFactory.Instance.GetWorklistType(worklist));
        }

        public void UpdateWorklist(Worklist worklist, WorklistAdminDetail detail, IPersistenceContext context)
        {
            worklist.Name = detail.Name;
            worklist.Description = detail.Description;
            
            // procedure groups
            worklist.ProcedureTypeGroupFilter.Values.Clear();
            if(detail.ProcedureTypeGroups != null)
            {
                worklist.ProcedureTypeGroupFilter.Values.AddAll(CollectionUtils.Map<ProcedureTypeGroupSummary, ProcedureTypeGroup>(
                    detail.ProcedureTypeGroups,
                    delegate(ProcedureTypeGroupSummary g) { return context.Load<ProcedureTypeGroup>(g.EntityRef, EntityLoadFlags.Proxy); }));
            }
            worklist.ProcedureTypeGroupFilter.IsEnabled = worklist.ProcedureTypeGroupFilter.Values.Count > 0;

            // facilities
            worklist.FacilityFilter.Values.Clear();
            if (detail.Facilities != null)
            {
                worklist.FacilityFilter.Values.AddAll(CollectionUtils.Map<FacilitySummary, Facility>(
                    detail.Facilities,
                    delegate(FacilitySummary f) { return context.Load<Facility>(f.FacilityRef, EntityLoadFlags.Proxy); }));
            }
            worklist.FacilityFilter.IncludeWorkingFacility = detail.FilterByWorkingFacility;
            worklist.FacilityFilter.IsEnabled = worklist.FacilityFilter.Values.Count > 0 ||
                                                worklist.FacilityFilter.IncludeWorkingFacility;

            // patient classes
            worklist.PatientClassFilter.Values.Clear();
            if (detail.PatientClasses != null)
            {
                worklist.PatientClassFilter.Values.AddAll(CollectionUtils.Map<EnumValueInfo, PatientClassEnum>(
                    detail.PatientClasses,
                    delegate(EnumValueInfo value) { return EnumUtils.GetEnumValue<PatientClassEnum>(value, context); }));
            }
            worklist.PatientClassFilter.IsEnabled = worklist.PatientClassFilter.Values.Count > 0;

            // order priorities
            worklist.OrderPriorityFilter.Values.Clear();
            if (detail.OrderPriorities != null)
            {
                worklist.OrderPriorityFilter.Values.AddAll(CollectionUtils.Map<EnumValueInfo, OrderPriorityEnum>(
                    detail.OrderPriorities,
                    delegate(EnumValueInfo value) { return EnumUtils.GetEnumValue<OrderPriorityEnum>(value, context); }));
            }
            worklist.OrderPriorityFilter.IsEnabled = worklist.OrderPriorityFilter.Values.Count > 0;

            // portable
            if(detail.Portabilities != null)
            {
                // put them into a set to guarantee uniqueness, in case the client sent a non-unique list
                HashedSet<bool> set = new HashedSet<bool>(detail.Portabilities);

                // it only makes sense to enable this filter if the set contains exactly one value (true or false, but not both)
                worklist.PortableFilter.IsEnabled = set.Count == 1;
                worklist.PortableFilter.Value = CollectionUtils.FirstElement(set, false);
            }

            // time window filters
            worklist.TimeFilter.Value = new WorklistTimeRange();
            worklist.TimeFilter.Value.Start = CreateTimePoint(detail.StartTime);
            worklist.TimeFilter.Value.End = CreateTimePoint(detail.EndTime);
            worklist.TimeFilter.IsEnabled = worklist.TimeFilter.Value.Start != null || worklist.TimeFilter.Value.End != null;

            // process users
            worklist.Users.Clear();
            worklist.Users.AddAll(detail.Users);
        }

        public WorklistTimePoint CreateTimePoint(WorklistAdminDetail.TimePoint contract)
        {
            if (contract != null && (contract.FixedTime.HasValue || contract.RelativeTime.HasValue))
            {
                return contract.FixedTime.HasValue ?
                    new WorklistTimePoint(contract.FixedTime.Value, contract.Resolution) :
                    new WorklistTimePoint(contract.RelativeTime.Value, contract.Resolution);
            }
            return null;
        }
    }
}