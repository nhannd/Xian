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

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    internal class WorklistAdminAssembler
    {
        public WorklistClassSummary CreateClassSummary(Type worklistClass)
        {
            Type ptgClass = Worklist.GetProcedureTypeGroupClass(worklistClass);

            return new WorklistClassSummary(
                Worklist.GetClassName(worklistClass),
                Worklist.GetDisplayName(worklistClass),
                Worklist.GetCategory(worklistClass),
                Worklist.GetDescription(worklistClass),
                ptgClass == null ? null : ptgClass.Name,
                ptgClass == null ? null : TerminologyTranslator.Translate(ptgClass),
                Worklist.GetSupportsTimeFilter(worklistClass),
                Worklist.GetSupportsReportingStaffRoleFilter(worklistClass));
        }

        public WorklistAdminDetail GetWorklistDetail(Worklist worklist, IPersistenceContext context)
        {
            WorklistAdminDetail detail = new WorklistAdminDetail(worklist.GetRef(), worklist.Name, worklist.Description,
                CreateClassSummary(worklist.GetClass()));

            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler staffGroupAssembler = new StaffGroupAssembler();
            detail.OwnerStaff = worklist.Owner.IsStaffOwner ?
                staffAssembler.CreateStaffSummary(worklist.Owner.Staff, context) : null;
            detail.OwnerGroup = worklist.Owner.IsGroupOwner ?
                staffGroupAssembler.CreateSummary(worklist.Owner.Group) : null;

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

            if (worklist.PatientLocationFilter.IsEnabled)
            {
                LocationAssembler locationAssembler = new LocationAssembler();
                detail.PatientLocations = CollectionUtils.Map<Location, LocationSummary>(
                    worklist.PatientLocationFilter.Values,
                    delegate(Location l) { return locationAssembler.CreateLocationSummary(l); });
            }

            if (worklist.OrderPriorityFilter.IsEnabled)
            {
                detail.OrderPriorities = CollectionUtils.Map<OrderPriorityEnum, EnumValueInfo>(
                    worklist.OrderPriorityFilter.Values,
                    delegate(OrderPriorityEnum p) { return EnumUtils.GetEnumValueInfo(p); });
            }

            if (worklist.OrderingPractitionerFilter.IsEnabled)
            {
                ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();
                detail.OrderingPractitioners = CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary>(
                    worklist.OrderingPractitionerFilter.Values,
                    delegate(ExternalPractitioner p) { return assembler.CreateExternalPractitionerSummary(p, context); });
            }

            if (worklist.PortableFilter.IsEnabled)
            {
                detail.Portabilities = new List<bool>();
                detail.Portabilities.Add(worklist.PortableFilter.Value);
            }

            if (worklist.TimeFilter.IsEnabled && worklist.TimeFilter.Value != null)
            {
                if (worklist.TimeFilter.Value.Start != null)
                    detail.StartTime = CreateTimePointContract(worklist.TimeFilter.Value.Start);
                if (worklist.TimeFilter.Value.End != null)
                    detail.EndTime = CreateTimePointContract(worklist.TimeFilter.Value.End);
            }

            detail.StaffSubscribers = CollectionUtils.Map<Staff, StaffSummary>(worklist.StaffSubscribers,
                delegate(Staff staff)
                {
                    return staffAssembler.CreateStaffSummary(staff, context);
                });

            detail.GroupSubscribers = CollectionUtils.Map<StaffGroup, StaffGroupSummary>(worklist.GroupSubscribers,
                delegate(StaffGroup group)
                {
                    return staffGroupAssembler.CreateSummary(group);
                });

            // Some ReportingWorklists can support staff role filters, if that is true for this worklist,
            // add those filters to the WorklistAdminDetail
            if (Worklist.GetSupportsReportingStaffRoleFilter(worklist.GetClass()))
                AppendReportingWorklistDetails(detail, worklist.As<ReportingWorklist>(), context);

            return detail;
        }

        public void AppendReportingWorklistDetails(WorklistAdminDetail detail, ReportingWorklist worklist, IPersistenceContext context)
        {
            if (worklist.InterpretedByStaffFilter.IsEnabled || worklist.InterpretedByStaffFilter.IncludeCurrentStaff)
                SetStaffListFromFilter(detail.InterpretedByStaff, worklist.InterpretedByStaffFilter, context);

            if (worklist.TranscribedByStaffFilter.IsEnabled || worklist.TranscribedByStaffFilter.IncludeCurrentStaff)
                SetStaffListFromFilter(detail.TranscribedByStaff, worklist.TranscribedByStaffFilter, context);

            if (worklist.VerifiedByStaffFilter.IsEnabled || worklist.VerifiedByStaffFilter.IncludeCurrentStaff)
                SetStaffListFromFilter(detail.VerifiedByStaff, worklist.VerifiedByStaffFilter, context);

            if (worklist.SupervisedByStaffFilter.IsEnabled || worklist.SupervisedByStaffFilter.IncludeCurrentStaff)
                SetStaffListFromFilter(detail.SupervisedByStaff, worklist.SupervisedByStaffFilter, context);
        }

        private static void SetStaffListFromFilter(WorklistAdminDetail.StaffList stafflist, WorklistStaffFilter filter, IPersistenceContext context)
        {
            StaffAssembler assembler = new StaffAssembler();
            stafflist.Staff = CollectionUtils.Map<Staff, StaffSummary>(
                filter.Values,
                delegate(Staff staff) { return assembler.CreateStaffSummary(staff, context); });
            stafflist.IncludeCurrentUser = filter.IncludeCurrentStaff;
        }

        public WorklistAdminDetail.TimePoint CreateTimePointContract(WorklistTimePoint tp)
        {
            return tp.IsFixed ? new WorklistAdminDetail.TimePoint(tp.FixedValue, tp.Resolution) :
                new WorklistAdminDetail.TimePoint(tp.RelativeValue, tp.Resolution);
        }

        public WorklistAdminSummary GetWorklistSummary(Worklist worklist, IPersistenceContext context)
        {
            bool isStatic = Worklist.GetIsStatic(worklist.GetClass());

            StaffAssembler staffAssembler = new StaffAssembler();
            StaffGroupAssembler groupAssembler = new StaffGroupAssembler();
            return new WorklistAdminSummary(
                isStatic ? null : worklist.GetRef(),
                isStatic ? Worklist.GetDisplayName(worklist.GetClass()) : worklist.Name,
                isStatic ? Worklist.GetDescription(worklist.GetClass()) : worklist.Description,
                CreateClassSummary(worklist.GetClass()),
                worklist.Owner.IsStaffOwner ? staffAssembler.CreateStaffSummary(worklist.Owner.Staff, context) : null,
                worklist.Owner.IsGroupOwner ? groupAssembler.CreateSummary(worklist.Owner.Group) : null);
        }

        public void UpdateWorklist(Worklist worklist, WorklistAdminDetail detail,
            bool updateSubscribers, IPersistenceContext context)
        {
            worklist.Name = detail.Name;
            worklist.Description = detail.Description;

            // do not update the worklist.Owner here!!! - once set, it should never be updated

            // procedure groups
            worklist.ProcedureTypeGroupFilter.Values.Clear();
            if (detail.ProcedureTypeGroups != null)
            {
                worklist.ProcedureTypeGroupFilter.Values.AddAll(CollectionUtils.Map<ProcedureTypeGroupSummary, ProcedureTypeGroup>(
                    detail.ProcedureTypeGroups,
                    delegate(ProcedureTypeGroupSummary g) { return context.Load<ProcedureTypeGroup>(g.ProcedureTypeGroupRef, EntityLoadFlags.Proxy); }));
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

            // patient locations
            worklist.PatientLocationFilter.Values.Clear();
            if (detail.PatientLocations != null)
            {
                worklist.PatientLocationFilter.Values.AddAll(CollectionUtils.Map<LocationSummary, Location>(
                    detail.PatientLocations,
                    delegate(LocationSummary f) { return context.Load<Location>(f.LocationRef, EntityLoadFlags.Proxy); }));
            }
            worklist.PatientLocationFilter.IsEnabled = worklist.PatientLocationFilter.Values.Count > 0;


            // order priorities
            worklist.OrderPriorityFilter.Values.Clear();
            if (detail.OrderPriorities != null)
            {
                worklist.OrderPriorityFilter.Values.AddAll(CollectionUtils.Map<EnumValueInfo, OrderPriorityEnum>(
                    detail.OrderPriorities,
                    delegate(EnumValueInfo value) { return EnumUtils.GetEnumValue<OrderPriorityEnum>(value, context); }));
            }
            worklist.OrderPriorityFilter.IsEnabled = worklist.OrderPriorityFilter.Values.Count > 0;

            // ordering practitioners
            worklist.OrderingPractitionerFilter.Values.Clear();
            if (detail.OrderingPractitioners != null)
            {
                worklist.OrderingPractitionerFilter.Values.AddAll(CollectionUtils.Map<ExternalPractitionerSummary, ExternalPractitioner>(
                    detail.OrderingPractitioners,
                    delegate(ExternalPractitionerSummary p)
                    {
                        return context.Load<ExternalPractitioner>(p.PractitionerRef, EntityLoadFlags.Proxy);
                    }));
            }
            worklist.OrderingPractitionerFilter.IsEnabled = worklist.OrderingPractitionerFilter.Values.Count > 0;

            // portable
            if (detail.Portabilities != null)
            {
                // put them into a set to guarantee uniqueness, in case the client sent a non-unique list
                HashedSet<bool> set = new HashedSet<bool>(detail.Portabilities);

                // it only makes sense to enable this filter if the set contains exactly one value (true or false, but not both)
                worklist.PortableFilter.IsEnabled = set.Count == 1;
                worklist.PortableFilter.Value = CollectionUtils.FirstElement(set, false);
            }

            // time window filters
            if (Worklist.GetSupportsTimeFilter(worklist.GetClass()))
            {
                WorklistTimePoint start = CreateTimePoint(detail.StartTime);
                WorklistTimePoint end = CreateTimePoint(detail.EndTime);
                if (start != null || end != null)
                {
                    worklist.TimeFilter.Value = new WorklistTimeRange(start, end);
                    worklist.TimeFilter.IsEnabled = true;
                }
                else
                    worklist.TimeFilter.IsEnabled = false;
            }

            // process subscriptions
            if (updateSubscribers)
            {
                worklist.StaffSubscribers.Clear();
                worklist.StaffSubscribers.AddAll(
                    CollectionUtils.Map<StaffSummary, Staff>(detail.StaffSubscribers,
                        delegate(StaffSummary summary)
                        {
                            return context.Load<Staff>(summary.StaffRef, EntityLoadFlags.Proxy);
                        }));

                worklist.GroupSubscribers.Clear();
                worklist.GroupSubscribers.AddAll(
                    CollectionUtils.Map<StaffGroupSummary, StaffGroup>(detail.GroupSubscribers,
                        delegate(StaffGroupSummary summary)
                        {
                            return context.Load<StaffGroup>(summary.StaffGroupRef, EntityLoadFlags.Proxy);
                        }));
            }

            // If the worklist supports staff role filters, process the filters provided.
            if (Worklist.GetSupportsReportingStaffRoleFilter(worklist.GetClass()))
                UpdateReportingWorklist(worklist.As<ReportingWorklist>(), detail, context);
        }

        public void UpdateReportingWorklist(ReportingWorklist worklist, WorklistAdminDetail detail, IPersistenceContext context)
        {
            UpdateStaffFilter(worklist.InterpretedByStaffFilter, detail.InterpretedByStaff, context);
            UpdateStaffFilter(worklist.TranscribedByStaffFilter, detail.TranscribedByStaff, context);
            UpdateStaffFilter(worklist.VerifiedByStaffFilter, detail.VerifiedByStaff, context);
            UpdateStaffFilter(worklist.SupervisedByStaffFilter, detail.SupervisedByStaff, context);
        }

        private static void UpdateStaffFilter(WorklistStaffFilter staffFilter, WorklistAdminDetail.StaffList stafflist, IPersistenceContext context)
        {
            staffFilter.Values.Clear();
            if (stafflist != null)
            {
                staffFilter.Values.AddAll(CollectionUtils.Map<StaffSummary, Staff>(
                    stafflist.Staff,
                    delegate(StaffSummary s) { return context.Load<Staff>(s.StaffRef, EntityLoadFlags.Proxy); }));

                staffFilter.IncludeCurrentStaff = stafflist.IncludeCurrentUser;
            }
            staffFilter.IsEnabled = staffFilter.Values.Count > 0 || staffFilter.IncludeCurrentStaff;
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