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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using System.Collections;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitEditorDetailsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitEditorDetailsComponent class
    /// </summary>
    [AssociateView(typeof(VisitEditorDetailsComponentViewExtensionPoint))]
    public class VisitDetailsEditorComponent : ApplicationComponent
    {
        private VisitDetail _visit;

        private readonly List<EnumValueInfo> _visitNumberAssigningAuthorityChoices;
        private readonly List<EnumValueInfo> _patientClassChoices;
        private readonly List<EnumValueInfo> _patientTypeChoices;
        private readonly List<EnumValueInfo> _admissionTypeChoices;
        private readonly List<EnumValueInfo> _visitStatusChoices;
		private readonly List<FacilitySummary> _facilityChoices;
		private readonly List<LocationSummary> _locationChoices;

		private List<EnumValueInfo> _ambulatoryStatusChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitDetailsEditorComponent(
                List<EnumValueInfo> visitNumberAssigningAuthorityChoices,
                List<EnumValueInfo> patientClassChoices,
                List<EnumValueInfo> patientTypeChoices,
                List<EnumValueInfo> admissionTypeChoices,
                List<EnumValueInfo> ambulatoryStatusChoices,
                List<EnumValueInfo> visitStatusChoices,
				List<FacilitySummary> facilityChoices,
				List<LocationSummary> locationChoices)
        {
            _visitNumberAssigningAuthorityChoices = visitNumberAssigningAuthorityChoices;
            _patientClassChoices = patientClassChoices;
            _patientTypeChoices = patientTypeChoices;
            _admissionTypeChoices = admissionTypeChoices;
            _ambulatoryStatusChoices = ambulatoryStatusChoices;
            _visitStatusChoices = visitStatusChoices;
        	_facilityChoices = facilityChoices;
        	_locationChoices = locationChoices;
        }

        public VisitDetail Visit
        {
            get { return _visit; }
            set { _visit = value; }
        }

        public override void Start()
        {
            if (_visit.VisitNumber == null)
            {
                _visit.VisitNumber = new CompositeIdentifierDetail();
                _visit.VisitNumber.AssigningAuthority = _visitNumberAssigningAuthorityChoices[0];
            }

            base.Start();
        }

        public override void Stop()
        {
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}
            base.Stop();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string VisitNumber
        {
            get { return _visit.VisitNumber.Id; }
            set
            {
                _visit.VisitNumber.Id = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public EnumValueInfo VisitNumberAssigningAuthority
        {
            get { return _visit.VisitNumber.AssigningAuthority; }
            set
            {
                _visit.VisitNumber.AssigningAuthority = value;
                this.Modified = true;
            }
        }

        public IList VisitNumberAssigningAuthorityChoices
        {
            get { return this._visitNumberAssigningAuthorityChoices; }
        }

        public DateTime? AdmitDateTime
        {
            get { return _visit.AdmitTime; }
            set
            {
                _visit.AdmitTime = value;
                this.Modified = true;
            }
        }

        public DateTime? DischargeDateTime
        {
            get { return _visit.DischargeTime; }
            set
            {
                _visit.DischargeTime = value;
                this.Modified = true;
            }
        }

        public string DischargeDisposition
        {
            get { return _visit.DischargeDisposition; }
            set
            {
                _visit.DischargeDisposition = value;
                this.Modified = true;
            }
        }

        public string PreAdmitNumber
        {
            get { return _visit.PreadmitNumber; }
            set
            {
                _visit.PreadmitNumber = value;
                this.Modified = true;
            }
        }

        public bool Vip
        {
            get { return _visit.VipIndicator; }
            set
            {
                _visit.VipIndicator = value;
                this.Modified = true;
            }
        }

        public EnumValueInfo PatientClass
        {
            get { return _visit.PatientClass; }
            set
            {
				if (!Equals(value, _visit.PatientClass))
				{
					_visit.PatientClass = value;
					this.Modified = true;
				}
			}
        }

        public IList PatientClassChoices
        {
            get { return _patientClassChoices; }
        }

		public EnumValueInfo PatientType
        {
            get { return _visit.PatientType; }
            set
            {
				if (!Equals(value, _visit.PatientType))
				{
					_visit.PatientType = value;
					this.Modified = true;
				}
			}
        }

        public IList PatientTypeChoices
        {
            get { return _patientTypeChoices; }
        }

		public EnumValueInfo AdmissionType
        {
            get { return _visit.AdmissionType; }
            set
            {
				if (!Equals(value, _visit.AdmissionType))
				{
					_visit.AdmissionType = value;
					this.Modified = true;
				}
			}
        }

        public IList AdmissionTypeChoices
        {
            get { return _admissionTypeChoices; }
        }

		public IList AmbulatoryStatusChoices
		{
			get { return _ambulatoryStatusChoices; }
		}

		public EnumValueInfo AmbulatoryStatus
		{
			get { return CollectionUtils.FirstElement(_visit.AmbulatoryStatuses); }
			set
			{
				if (!_visit.AmbulatoryStatuses.Contains(value))
				{
					_visit.AmbulatoryStatuses.Clear();
					_visit.AmbulatoryStatuses.Add(value);
					this.Modified = true;
				}
			}
		}

        [ValidateNotNull]
        public EnumValueInfo VisitStatus
        {
            get { return _visit.Status; }
            set
            {
				if (!Equals(value, _visit.Status))
				{
					_visit.Status = value;
					this.Modified = true;
				}
			}
        }

		public IList VisitStatusChoices
        {
            get { return _visitStatusChoices; }
        }

    	public IList FacilityChoices
    	{
			get { return _facilityChoices; }
    	}

        [ValidateNotNull]
        public FacilitySummary Facility
    	{
			get { return _visit.Facility; }
			set
			{
				if(!Equals(value, _visit.Facility))
				{
					_visit.Facility = value;
					this.Modified = true;
				}
			}
    	}

		public string FormatFacility(object item)
		{
			FacilitySummary f = (FacilitySummary) item;
			return f.Name;
		}

		public IList CurrentLocationChoices
		{
			get { return _locationChoices; }
		}

		public LocationSummary CurrentLocation
		{
			get { return _visit.CurrentLocation; }
			set
			{
				if (!Equals(value, _visit.CurrentLocation))
				{
					_visit.CurrentLocation = value;
					this.Modified = true;
				}
			}
		}

		public string FormatCurrentLocation(object item)
		{
			LocationSummary l = (LocationSummary) item;
			return l.Name;
		}

        #endregion
    }
}
