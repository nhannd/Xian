using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Visit entity
    /// </summary>
	public partial class Visit : Entity
	{
        private void CustomInitialize()
        {
        }

        public void CopyFrom(Visit v)
        {
            this.VisitNumber.Id = v.VisitNumber.Id;
            this.VisitNumber.AssigningAuthority = v.VisitNumber.AssigningAuthority;
            this.VisitStatus = v.VisitStatus;
            this.AdmitDateTime = v.AdmitDateTime;
            this.PatientClass = v.PatientClass;
            this.PatientType = v.PatientType;
            this.AdmissionType = v.AdmissionType;
            this.Facility = v.Facility;
            this.DischargeDateTime = v.DischargeDateTime;
            this.DischargeDisposition = v.DischargeDisposition;
            this.VipIndicator = v.VipIndicator;
            this.AmbulatoryStatus = v.AmbulatoryStatus;
            this.PreadmitNumber = v.PreadmitNumber;

            foreach (VisitPractitioner vp in v.Practitioners)
            {
                VisitPractitioner practitioner = new VisitPractitioner();
                practitioner.CopyFrom(vp);
                this.Practitioners.Add(practitioner);
            }

            foreach (VisitLocation vl in v.Locations)
            {
                VisitLocation location = new VisitLocation();
                location.CopyFrom(vl);
                this.Locations.Add(location);
            }
        }
	}
}
