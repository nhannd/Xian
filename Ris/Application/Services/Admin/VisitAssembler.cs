using System;
using System.Collections.Generic;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class VisitAssembler
    {
        public VisitSummary CreateVisitSummary(Visit visit, IPersistenceContext context)
        {
            VisitSummary summary = new VisitSummary();

            summary.entityRef = visit.GetRef();
            summary.Patient = visit.Patient.GetRef();

            summary.VisitNumberAssigningAuthority = visit.VisitNumber.AssigningAuthority;
            summary.VisitNumberId = visit.VisitNumber.Id;

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

            detail.Patient = visit.Patient.GetRef();
            detail.VisitNumberAssigningAuthority = visit.VisitNumber.AssigningAuthority;
            detail.VisitNumberId = visit.VisitNumber.Id;
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

        public void UpdateVisit(Visit visit, VisitDetail detail, IPersistenceContext context)
        {
            // TODO: add validation and throw RequestValidationException as necessary

            visit.Patient = context.Load<Patient>(detail.Patient, EntityLoadFlags.Proxy);
            visit.VisitNumber.Id = detail.VisitNumberId;
            visit.VisitNumber.AssigningAuthority = detail.VisitNumberAssigningAuthority;

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
