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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Healthcare {
    
    public class PatientClassConversionException : Exception
    {
    }

    /// <summary>
    /// Visit entity
    /// </summary>
    [UniqueKey("VisitNumber", new string[] {"VisitNumber.Id", "VisitNumber.AssigningAuthority"})]
	public partial class Visit : Entity
	{
        private void CustomInitialize()
        {
        }

		public virtual void CopyFrom(Visit v)
		{
			this.VisitNumber.Id = v.VisitNumber.Id;
			this.VisitNumber.AssigningAuthority = v.VisitNumber.AssigningAuthority;
			this.Status = v.Status;
			this.AdmitTime = v.AdmitTime;
			this.PatientClass = v.PatientClass;
			this.PatientType = v.PatientType;
			this.AdmissionType = v.AdmissionType;
			this.Facility = v.Facility;
			this.DischargeTime = v.DischargeTime;
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

			foreach (System.Collections.Generic.KeyValuePair<string,string> pair in v.ExtendedProperties)
			{
				this.ExtendedProperties.Add(pair);
			}
		}

        public virtual void Cancel()
        {
            this.Status = VisitStatus.CX;
        }

        public virtual void CancelPreAdmit()
        {
            this.Status = VisitStatus.PC;
        }

        /// <summary>
        /// Infers VisitStatus from AdmitTime and DischargeTime.  
        /// </summary>
        public virtual void InferVisitStatus()
        {
            if (this.AdmitTime.HasValue) this.Status = VisitStatus.AA;
            if (this.DischargeTime.HasValue) this.Status = VisitStatus.DC;
        }

        public virtual void Discharge(DateTime dischargeDateTime, string dischargeDispostion)
        {
            if (this.Status != VisitStatus.DC && this.Status != VisitStatus.CX)
            {
                this.Status = VisitStatus.DC;
                this.DischargeTime = dischargeDateTime;
                this.DischargeDisposition = dischargeDispostion;
            }
        }

        public virtual void PopCurrentLocation()
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

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </para>
		/// </remarks>
		/// <param name="minutes"></param>
		public virtual void TimeShift(int minutes)
		{
			_admitTime = _admitTime.HasValue ? _admitTime.Value.AddMinutes(minutes) : _admitTime;
			_dischargeTime = _dischargeTime.HasValue ? _dischargeTime.Value.AddMinutes(minutes) : _dischargeTime;

			foreach (VisitPractitioner prac in _practitioners)
			{
				prac.StartTime = prac.StartTime.HasValue ? prac.StartTime.Value.AddMinutes(minutes) : prac.StartTime;
				prac.EndTime = prac.EndTime.HasValue ? prac.EndTime.Value.AddMinutes(minutes) : prac.EndTime;
			}

			foreach (VisitLocation loc in _locations)
			{
				loc.StartTime = loc.StartTime.HasValue ? loc.StartTime.Value.AddMinutes(minutes) : loc.StartTime;
				loc.EndTime = loc.EndTime.HasValue ? loc.EndTime.Value.AddMinutes(minutes) : loc.EndTime;
				
			}
		}
    }
}
