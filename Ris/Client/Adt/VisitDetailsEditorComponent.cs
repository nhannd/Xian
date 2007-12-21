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

using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using System.Collections;

namespace ClearCanvas.Ris.Client.Adt
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
        private readonly List<EnumValueInfo> _ambulatoryStatusChoices;
        private readonly List<EnumValueInfo> _visitStatusChoices;
        private List<FacilitySummary> _facilityChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitDetailsEditorComponent(
                List<EnumValueInfo> visitNumberAssigningAuthorityChoices,
                List<EnumValueInfo> patientClassChoices,
                List<EnumValueInfo> patientTypeChoices,
                List<EnumValueInfo> admissionTypeChoices,
                List<EnumValueInfo> ambulatoryStatusChoices,
                List<EnumValueInfo> visitStatusChoices)
        {
            _visitNumberAssigningAuthorityChoices = visitNumberAssigningAuthorityChoices;
            _patientClassChoices = patientClassChoices;
            _patientTypeChoices = patientTypeChoices;
            _admissionTypeChoices = admissionTypeChoices;
            _ambulatoryStatusChoices = ambulatoryStatusChoices;
            _visitStatusChoices = visitStatusChoices;
        }

        public VisitDetail Visit
        {
            get { return _visit; }
            set { _visit = value; }
        }

        public override void Start()
        {
            Platform.GetService<IFacilityAdminService>(
                delegate(IFacilityAdminService service)
                {
                    ///TODO: expose facility in the UI
                    ListAllFacilitiesResponse listResponse = service.ListAllFacilities(new ListAllFacilitiesRequest());
                    _facilityChoices = listResponse.Facilities;

                    if (listResponse.Facilities.Count == 0)
                    {
                        GetFacilityEditFormDataResponse formResponse = service.GetFacilityEditFormData(new GetFacilityEditFormDataRequest());
                        EnumValueInfo randomInformationAuthority = RandomUtils.ChooseRandom(formResponse.InformationAuthorityChoices);

                        AddFacilityResponse addResponse = service.AddFacility(new AddFacilityRequest(new FacilityDetail("", "Test Facility", randomInformationAuthority)));
                        _visit.Facility = addResponse.Facility;
                        _facilityChoices.Add(addResponse.Facility);
                    }
                });

            if (_visit.VisitNumber == null)
            {
                _visit.VisitNumber = new CompositeIdentifierDetail();
                _visit.VisitNumber.AssigningAuthority = _visitNumberAssigningAuthorityChoices[0];
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region DataBinding Properties

        public string VisitNumber
        {
            get { return _visit.VisitNumber.Id; }
            set
            {
                _visit.VisitNumber.Id = value;
                this.Modified = true;
            }
        }

        #region AssigningAuthority
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

        #endregion

        public DateTime? AdmitDateTime
        {
            get { return _visit.AdmitDateTime; }
            set
            {
                _visit.AdmitDateTime = value;
                this.Modified = true;
            }
        }

        public DateTime? DischargeDateTime
        {
            get { return _visit.DischargeDateTime; }
            set
            {
                _visit.DischargeDateTime = value;
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

        #region PatientClass
        public string PatientClass
        {
            get { return _visit.PatientClass.Value; }
            set
            {
                _visit.PatientClass = EnumValueUtils.MapDisplayValue(_patientClassChoices, value);
                this.Modified = true;
            }
        }

        public List<string> PatientClassChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_patientClassChoices); }
        }
        #endregion

        #region PatientType
        public string PatientType
        {
            get { return _visit.PatientType.Value; }
            set
            {
                _visit.PatientType = EnumValueUtils.MapDisplayValue(_patientTypeChoices, value);
                this.Modified = true;
            }
        }

        public List<string> PatientTypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_patientTypeChoices); }
        }
        #endregion

        #region AdmissionType
        public string AdmissionType
        {
            get { return _visit.AdmissionType.Value; }
            set
            {
                _visit.AdmissionType = EnumValueUtils.MapDisplayValue(_admissionTypeChoices, value);
                this.Modified = true;
            }
        }

        public List<string> AdmissionTypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_admissionTypeChoices); }
        }
        #endregion

        #region AmbulatoryStatus
        //public string AmbulatoryStatus
        //{
        //    get { return _visit.AmbulatoryStatus == null ? "" : _visit.AmbulatoryStatus.Value; }
        //    set
        //    {
        //        _visit.AmbulatoryStatus = (value == "") ? null :
        //            CollectionUtils.SelectFirst<EnumValueInfo>(_ambulatoryStatusChoices,
        //            delegate(EnumValueInfo e) { return e.Value == Value; });
        //        this.Modified = true;
        //    }
        //}

        //public List<string> AmbulatoryStatusChoices
        //{
        //    get { return this._ambulatoryStatusChoices; }
        //}
        #endregion

        #region VisitStatus
        public String VisitStatus
        {
            get { return _visit.Status.Value; }
            set
            {
                _visit.Status = EnumValueUtils.MapDisplayValue(_visitStatusChoices, value);
                this.Modified = true;
            }
        }

        public List<string> VisitStatusChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_visitStatusChoices); }
        }
        #endregion

        #endregion
    }
}
