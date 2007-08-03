using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {
    
    public class PatientClassConversionException : Exception
    {
    }

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
            //this.AmbulatoryStatus = v.AmbulatoryStatus;
            this.PreadmitNumber = v.PreadmitNumber;

            foreach (AmbulatoryStatusEnum a in v.AmbulatoryStatuses)
            {
                this.AmbulatoryStatuses.Add(a);
            }

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

        public void Cancel()
        {
            this.VisitStatus = VisitStatus.CX;
        }

        public void CancelPreAdmit()
        {
            this.VisitStatus = VisitStatus.PC;
        }

        /// <summary>
        /// Infers VisitStatus from AdmitDateTime and DischargeDateTime.  
        /// </summary>
        public void InferVisitStatus()
        {
            if (this.AdmitDateTime.HasValue) this.VisitStatus = VisitStatus.AA;
            if (this.DischargeDateTime.HasValue) this.VisitStatus = VisitStatus.DC;
        }

        public void Discharge(DateTime dischargeDateTime, string dischargeDispostion)
        {
            if (this.VisitStatus != VisitStatus.DC && this.VisitStatus != VisitStatus.CX)
            {
                this.VisitStatus = VisitStatus.DC;
                this.DischargeDateTime = dischargeDateTime;
                this.DischargeDisposition = dischargeDispostion;
            }
        }

        public void PopCurrentLocation()
        {
            DateTime previousCurrentDate = DateTime.MinValue;
            VisitLocation current = null;
            VisitLocation previousCurrent = null;
            
            foreach (VisitLocation vl in this.Locations)
            {
                if (vl.Role == VisitLocationRole.CR)
                {
                    if (vl.EndTime == null)
                    {
                        current = vl;
                    }
                    else if (vl.EndTime > previousCurrentDate)
                    {
                        previousCurrent = vl;
                    }
                }
            }

            if (current != null && previousCurrent != null)
            {
                this.Locations.Remove(current);
                previousCurrent.EndTime = null;
            }
            else
            {
            }
        }
    }
}
