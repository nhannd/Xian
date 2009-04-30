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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class VisitAssembler
    {
        public VisitSummary CreateVisitSummary(Visit visit, IPersistenceContext context)
        {
            VisitSummary summary = new VisitSummary();

            summary.VisitRef = visit.GetRef();
            summary.PatientRef = visit.Patient.GetRef();

            summary.VisitNumber = CreateVisitNumberDetail(visit.VisitNumber);

            summary.AdmissionType = EnumUtils.GetEnumValueInfo(visit.AdmissionType);
            summary.PatientClass = EnumUtils.GetEnumValueInfo(visit.PatientClass);
            summary.PatientType = EnumUtils.GetEnumValueInfo(visit.PatientType);
            summary.Status = EnumUtils.GetEnumValueInfo(visit.Status, context);

            summary.AdmitTime = visit.AdmitTime;
            summary.DischargeTime = visit.DischargeTime;

			FacilityAssembler facilityAssembler = new FacilityAssembler();
        	summary.Facility = visit.Facility == null ? null : facilityAssembler.CreateFacilitySummary(visit.Facility);

			LocationAssembler locationAssembler = new LocationAssembler();
			summary.CurrentLocation = visit.CurrentLocation == null ? null : locationAssembler.CreateLocationSummary(visit.CurrentLocation);

            return summary;
        }

        public VisitDetail CreateVisitDetail(Visit visit, IPersistenceContext context)
        {
            VisitDetail detail = new VisitDetail();
            detail.VisitRef = visit.GetRef();
            detail.PatientRef = visit.Patient.GetRef();
            detail.VisitNumber = CreateVisitNumberDetail(visit.VisitNumber);
            detail.AdmissionType = EnumUtils.GetEnumValueInfo(visit.AdmissionType);
            detail.PatientClass = EnumUtils.GetEnumValueInfo(visit.PatientClass);
            detail.PatientType = EnumUtils.GetEnumValueInfo(visit.PatientType);
            detail.Status = EnumUtils.GetEnumValueInfo(visit.Status, context);


            detail.AdmitTime = visit.AdmitTime;
            detail.DischargeTime = visit.DischargeTime;

            detail.DischargeDisposition = visit.DischargeDisposition;

            detail.Facility = new FacilityAssembler().CreateFacilitySummary(visit.Facility);
			detail.CurrentLocation = visit.CurrentLocation == null ? null : new LocationAssembler().CreateLocationSummary(visit.CurrentLocation);
            
            detail.Locations = new List<VisitLocationDetail>();
            foreach (VisitLocation vl in visit.Locations)
            {
                detail.Locations.Add(CreateVisitLocationDetail(vl, context));
            }

            detail.Practitioners = new List<VisitPractitionerDetail>();
            foreach (VisitPractitioner vp in visit.Practitioners)
            {
                detail.Practitioners.Add(CreateVisitPractitionerDetail(vp, context));
            }

            detail.AmbulatoryStatuses = new List<EnumValueInfo>();
            foreach (AmbulatoryStatusEnum ambulatoryStatus in visit.AmbulatoryStatuses)
	        {
                detail.AmbulatoryStatuses.Add(EnumUtils.GetEnumValueInfo(ambulatoryStatus));
	        }   
         
            detail.PreadmitNumber = visit.PreadmitNumber;
            detail.VipIndicator = visit.VipIndicator;

        	detail.ExtendedProperties = new Dictionary<string, string>(visit.ExtendedProperties);

			return detail;
        }

        public CompositeIdentifierDetail CreateVisitNumberDetail(VisitNumber vn)
        {
            return new CompositeIdentifierDetail(vn.Id, EnumUtils.GetEnumValueInfo(vn.AssigningAuthority));
        }

        public void UpdateVisit(Visit visit, VisitDetail detail, IPersistenceContext context)
        {
            visit.Patient = context.Load<Patient>(detail.PatientRef, EntityLoadFlags.Proxy);
            visit.VisitNumber.Id = detail.VisitNumber.Id;
            visit.VisitNumber.AssigningAuthority = EnumUtils.GetEnumValue<InformationAuthorityEnum>(detail.VisitNumber.AssigningAuthority, context);

            visit.AdmissionType = EnumUtils.GetEnumValue<AdmissionTypeEnum>(detail.AdmissionType, context);
            visit.PatientClass = EnumUtils.GetEnumValue<PatientClassEnum>(detail.PatientClass, context);
            visit.PatientType = EnumUtils.GetEnumValue<PatientTypeEnum>(detail.PatientType, context);
            visit.Status = EnumUtils.GetEnumValue<VisitStatus>(detail.Status);

            visit.AdmitTime = detail.AdmitTime;
            visit.DischargeTime = detail.DischargeTime;
            visit.DischargeDisposition = detail.DischargeDisposition;

			visit.Facility = detail.Facility == null ? null :
				context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
			visit.CurrentLocation = detail.CurrentLocation == null ? null :
				context.Load<Location>(detail.CurrentLocation.LocationRef, EntityLoadFlags.Proxy);

            visit.Locations.Clear();
            foreach (VisitLocationDetail vlDetail in detail.Locations)
            {
                visit.Locations.Add(new VisitLocation(
                    context.Load<Location>(vlDetail.Location.LocationRef, EntityLoadFlags.Proxy),
                    EnumUtils.GetEnumValue<VisitLocationRole>(vlDetail.Role),
                    vlDetail.StartTime,
                    vlDetail.EndTime));
            }

            visit.Practitioners.Clear();
            foreach (VisitPractitionerDetail vpDetail in detail.Practitioners)
            {
                visit.Practitioners.Add(new VisitPractitioner(
                    context.Load<ExternalPractitioner>(vpDetail.Practitioner.PractitionerRef, EntityLoadFlags.Proxy),
                    EnumUtils.GetEnumValue<VisitPractitionerRole>(vpDetail.Role),
                    vpDetail.StartTime,
                    vpDetail.EndTime));
            }

            visit.AmbulatoryStatuses.Clear();
            foreach (EnumValueInfo ambulatoryStatus in detail.AmbulatoryStatuses)
            {
                visit.AmbulatoryStatuses.Add(EnumUtils.GetEnumValue<AmbulatoryStatusEnum>(ambulatoryStatus, context));   
            }

			// explicitly copy each pair, so that we don't remove any properties that the client may have removed
			foreach (KeyValuePair<string, string> pair in detail.ExtendedProperties)
			{
				visit.ExtendedProperties[pair.Key] = pair.Value;
			}
		}

        private VisitLocationDetail CreateVisitLocationDetail(VisitLocation vl, IPersistenceContext context)
        {
            VisitLocationDetail detail = new VisitLocationDetail();

            detail.Location = new LocationAssembler().CreateLocationSummary(vl.Location);
            detail.Role = EnumUtils.GetEnumValueInfo(vl.Role, context);

            detail.StartTime = vl.StartTime;
            detail.EndTime = vl.EndTime;

            return detail;
        }

        private VisitPractitionerDetail CreateVisitPractitionerDetail(VisitPractitioner vp, IPersistenceContext context)
        {
            VisitPractitionerDetail detail = new VisitPractitionerDetail();
            
            detail.Practitioner = new ExternalPractitionerAssembler().CreateExternalPractitionerSummary(vp.Practitioner, context);

            detail.Role = EnumUtils.GetEnumValueInfo(vp.Role, context);

            detail.StartTime = vp.StartTime;
            detail.EndTime = vp.EndTime;

            return detail;
        }
    }
}
