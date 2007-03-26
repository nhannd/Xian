using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitEditorDetailsComponent"/>
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

        private List<string> _visitNumberAssigningAuthorityChoices;
        private List<EnumValueInfo> _patientClassChoices;
        private List<EnumValueInfo> _patientTypeChoices;
        private List<EnumValueInfo> _admissionTypeChoices;
        private List<EnumValueInfo> _ambulatoryStatusChoices;
        private List<EnumValueInfo> _visitStatusChoices;
        private List<FacilitySummary> _facilityChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitDetailsEditorComponent(
                List<string> visitNumberAssigningAuthorityChoices,
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
            try
            {
                Platform.GetService<IFacilityAdminService>(
                    delegate(IFacilityAdminService service)
                    {
                        ///TODO: expose facility in the UI
                        ListAllFacilitiesResponse listResponse = service.ListAllFacilities(new ListAllFacilitiesRequest());
                        _facilityChoices = listResponse.Facilities;

                        if (listResponse.Facilities.Count == 0)
                        {
                            AddFacilityResponse addResponse = service.AddFacility(new AddFacilityRequest(new FacilityDetail("", "Test Facility")));
                            _visit.Facility = addResponse.Facility;
                            _facilityChoices.Add(addResponse.Facility);
                        }
                    });

                if (_visit.VisitNumberAssigningAuthority == null)
                {
                    _visit.VisitNumberAssigningAuthority = _visitNumberAssigningAuthorityChoices[0];
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
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
            get { return _visit.VisitNumberId; }
            set
            {
                _visit.VisitNumberId = value;
                this.Modified = true;
            }
        }

        #region AssigningAuthority
        public string VisitNumberAssigningAuthority
        {
            get { return _visit.VisitNumberAssigningAuthority; }
            set
            {
                _visit.VisitNumberAssigningAuthority = value;
                this.Modified = true;
            }
        }

        public List<string> VisitNumberAssigningAuthorityChoices
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
            get { return _visit.PatientClass == null ? "" : _visit.PatientClass.Value; }
            set
            {
                _visit.PatientClass = (value == "") ? null :
                    CollectionUtils.SelectFirst<EnumValueInfo>(_patientClassChoices,
                    delegate(EnumValueInfo e) { return e.Value == value; });
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
            get { return _visit.PatientType == null ? "" : _visit.PatientType.Value; }
            set
            {
                _visit.PatientType = (value == "") ? null :
                    CollectionUtils.SelectFirst<EnumValueInfo>(_patientTypeChoices,
                    delegate(EnumValueInfo e) { return e.Value == value; });
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
            get { return _visit.AdmissionType == null ? "" : _visit.AdmissionType.Value; }
            set
            {
                _visit.AdmissionType = (value == "") ? null :
                    CollectionUtils.SelectFirst<EnumValueInfo>(_admissionTypeChoices,
                    delegate(EnumValueInfo e) { return e.Value == value; });
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
            get { return _visit.Status == null ? "" : _visit.Status.Value; }
            set
            {
                _visit.Status = (value == "") ? null :
                    CollectionUtils.SelectFirst<EnumValueInfo>(_visitStatusChoices,
                    delegate(EnumValueInfo e) { return e.Value == value; });
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
