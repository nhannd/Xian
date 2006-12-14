using System;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    public class VisitTable : Table<Visit>
    {
        public VisitTable()
        {
            IAdtService service = ApplicationContext.GetService<IAdtService>();

            PatientClassEnumTable patientClasses = service.GetPatientClassEnumTable();
            PatientTypeEnumTable patientTypes = service.GetPatientTypeEnumTable();
            AdmissionTypeEnumTable admissionTypes = service.GetAdmissionTypeEnumTable();
            AmbulatoryStatusEnumTable ambulatoryStatuses = service.GetAmbulatoryStatusEnumTable();
            VisitStatusEnumTable visitStatuses = service.GetVisitStatusEnumTable();

            this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnVisitNumber,
                delegate(Visit v) { return v.VisitNumber.Format(); },
                1.0f));

            //Visit type description
            this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnVisitType,
                delegate(Visit v)
                {
                    return patientClasses[v.PatientClass].Value + (v.PatientType != PatientType.X ? " - " + patientTypes[v.PatientType].Value : v.AdmissionType != AdmissionType.X ? " - " + admissionTypes[v.AdmissionType].Value : "");
                },
                1.0f));
            
            ////number
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnNumber,
            //    delegate(Visit v) { return v.VisitNumber.Id; },
            //    1.0f));
            ////site
            //this.Columns.Add(new TableColumn<Visit, string>("Assigning Authority",
            //    delegate(Visit v) { return v.VisitNumber.AssigningAuthority; },
            //    1.0f));

            //status
            this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnStatus,
                delegate(Visit v) { return visitStatuses[v.VisitStatus].Value; },
                1.0f));

            //admit date/time
            this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnAdmitDateTime,
                delegate(Visit v) { return Format.Date(v.AdmitDateTime); },
                1.0f));

            ////Patient class
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnPatientClass,
            //    delegate(Visit v) { return patientClasses[v.PatientClass].Value; },
            //    1.0f));

            ////patient type
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnPatientType,
            //    delegate(Visit v) { return patientTypes[v.PatientType].Value; },
            //    1.0f));

            ////admission type
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnAdmissionType,
            //    delegate(Visit v) { return admissionTypes[v.AdmissionType].Value; },
            //    1.0f));

            //facility
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnFacility,
            //    delegate(Visit v) { return v.Facility != null ? v.Facility.Name : ""; },
            //    1.0f));ime

            //discharge datetime
            this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnDischargeDateTime,
                delegate(Visit v) { return Format.Date(v.DischargeDateTime); },
                1.0f));

            ////discharge disposition
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnDischargeDisposition,
            //    delegate(Visit v) { return v.DischargeDisposition; },
            //    1.0f));

            //current location
            //_visits.Columns.Add(new TableColumn<Visit, string>(SR.ColumnCurrentLocation,
            //    delegate(Visit v) { return v.CurrentLocation.Format(); },
            //    1.0f));

            //practitioners
            //_visits.Columns.Add(new TableColumn<Visit, string>("Some Practitioner",
            //    delegate(Visit v) { return; },
            //    1.0f));
            
            ////VIP
            //this.Columns.Add(new TableColumn<Visit, bool>("VIP?",
            //    delegate(Visit v) { return v.VipIndicator; },
            //    1.0f));

            ////Ambulatory status
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnAmbulatoryStatus,
            //    delegate(Visit v) { return ambulatoryStatuses[v.AmbulatoryStatus].Value; },
            //    1.0f));
            
            ////preadmit number
            //this.Columns.Add(new TableColumn<Visit, string>(SR.ColumnPreAdmitNumber,
            //    delegate(Visit v) { return v.PreadmitNumber; },
            //    1.0f));

        }
    }
}
