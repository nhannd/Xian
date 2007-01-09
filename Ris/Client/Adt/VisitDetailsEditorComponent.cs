using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;

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
        private Visit _visit;

        private PatientClassEnumTable _patientClasses;
        private PatientTypeEnumTable _patientTypes;
        private AdmissionTypeEnumTable _admissionTypes;
        private AmbulatoryStatusEnumTable _ambulatoryStatuses;
        private VisitStatusEnumTable _visitStatuses;

        private string[] _dummySiteChoices = new string[] { "UHN", "MSH", "SiteA", "SiteB", "SiteC", "SiteD", "SiteE", "SiteF" };

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitDetailsEditorComponent()
        {
        }

        public Visit Visit
        {
            get { return _visit; }
            set { _visit = value; }
        }

        public override void Start()
        {
            IAdtService service = ApplicationContext.GetService<IAdtService>();

            _patientClasses = service.GetPatientClassEnumTable();
            _patientTypes = service.GetPatientTypeEnumTable();
            _admissionTypes = service.GetAdmissionTypeEnumTable();
            _ambulatoryStatuses = service.GetAmbulatoryStatusEnumTable();
            _visitStatuses = service.GetVisitStatusEnumTable();

            if (_visit.VisitNumber.AssigningAuthority == null)
            {
                _visit.VisitNumber.AssigningAuthority = _dummySiteChoices[0];
            }

            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
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
        public string VisitNumberAssigningAuthority
        {
            get { return _visit.VisitNumber.AssigningAuthority; }
            set
            {
                _visit.VisitNumber.AssigningAuthority = value;
                this.Modified = true;
            }
        }

        public string[] VisitNumberAssigningAuthorityChoices
        {
            get { return _dummySiteChoices; }
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
            get { return _patientClasses[_visit.PatientClass].Value; }
            set
            {
                _visit.PatientClass = _patientClasses[value].Code;
                this.Modified = true;
            }
        }

        public string[] PatientClassChoices
        {
            get { return _patientClasses.Values; }
        }
        #endregion

        #region PatientType
        public string PatientType
        {
            get { return _patientTypes[_visit.PatientType].Value; }
            set
            {
                _visit.PatientType = _patientTypes[value].Code;
                this.Modified = true;
            }
        }

        public string[] PatientTypeChoices
        {
            get { return _patientTypes.Values; }
        }
        #endregion

        #region AdmissionType
        public string AdmissionType
        {
            get { return _admissionTypes[_visit.AdmissionType].Value; }
            set
            {
                _visit.AdmissionType = _admissionTypes[value].Code;
                this.Modified = true;
            }
        }

        public string[] AdmissionTypeChoices
        {
            get { return _admissionTypes.Values; }
        }
        #endregion

        //#region AmbulatoryStatus
        //public string AmbulatoryStatus
        //{
        //    get { return _ambulatoryStatuses[_visit.AmbulatoryStatus].Value; }
        //    set
        //    {
        //        _visit.AmbulatoryStatus = _ambulatoryStatuses[value].Code;
        //        this.Modified = true;
        //    }
        //}

        //public string[] AmbulatoryStatusChoices
        //{
        //    get { return _ambulatoryStatuses.Values; }
        //}
        //#endregion

        #region VisitStatus
        public String VisitStatus
        {
            get { return _visitStatuses[_visit.VisitStatus].Value; }
            set
            {
                _visit.VisitStatus = _visitStatuses[value].Code;
                this.Modified = true;
            }
        }

        public string[] VisitStatusChoices
        {
            get { return _visitStatuses.Values; }
        }
        #endregion

        #endregion
    }
}
