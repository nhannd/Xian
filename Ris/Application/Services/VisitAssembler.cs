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

            summary.AdmissionType = visit.AdmissionType.Value;
            summary.PatientClass = visit.PatientClass.Value;
            summary.PatientType = visit.PatientType.Value;
            summary.Status = EnumUtils.GetValue(visit.VisitStatus, context);

            summary.AdmitDateTime = visit.AdmitDateTime;
            summary.DischargeDateTime = visit.DischargeDateTime;

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
            detail.Status = EnumUtils.GetEnumValueInfo(visit.VisitStatus, context);


            detail.AdmitDateTime = visit.AdmitDateTime;
            detail.DischargeDateTime = visit.DischargeDateTime;

            detail.DischargeDisposition = visit.DischargeDisposition;

            detail.Facility = new FacilityAssembler().CreateFacilitySummary(visit.Facility);
            
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

            return detail;
        }

        public CompositeIdentifierDetail CreateVisitNumberDetail(VisitNumber vn)
        {
            return new CompositeIdentifierDetail(vn.Id, EnumUtils.GetEnumValueInfo(vn.AssigningAuthority));
        }

        public void UpdateVisit(Visit visit, VisitDetail detail, IPersistenceContext context)
        {
            // TODO: add validation and throw RequestValidationException as necessary

            visit.Patient = context.Load<Patient>(detail.PatientRef, EntityLoadFlags.Proxy);
            visit.VisitNumber.Id = detail.VisitNumber.Id;
            visit.VisitNumber.AssigningAuthority = EnumUtils.GetEnumValue<InformationAuthorityEnum>(detail.VisitNumber.AssigningAuthority, context);

            visit.AdmissionType = EnumUtils.GetEnumValue<AdmissionTypeEnum>(detail.AdmissionType, context);
            visit.PatientClass = EnumUtils.GetEnumValue<PatientClassEnum>(detail.PatientClass, context);
            visit.PatientType = EnumUtils.GetEnumValue<PatientTypeEnum>(detail.PatientType, context);
            visit.VisitStatus = EnumUtils.GetEnumValue<VisitStatus>(detail.Status);

            visit.AdmitDateTime = detail.AdmitDateTime;
            visit.DischargeDateTime = detail.DischargeDateTime;
            visit.DischargeDisposition = detail.DischargeDisposition;

            if (detail.Facility != null)
            {
                visit.Facility = context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
            }
            else
            {
                throw new RequestValidationException("Visit requires a facility");
            }

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
