using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PatientSearchComponentViewExtensionPoint))]
    public class PatientSearchComponent : ApplicationComponent
    {
        private IWorkspace _patientAdminWorkspace;
        private PatientAdminComponent _patientAdminComponent;
        private IPatientAdminService _patientAdminService;

        private string _patientIdentifier;
        private PatientIdentifierType _patientIdentifierType;
        private string _familyName;
        private string _givenName;
        private Sex? _sex;
        private DateTime? _dateOfBirth;

        private bool _searchEnabled;
        private event EventHandler _searchEnabledChanged;
        
        public PatientSearchComponent()
        {

        }

        public override void Start()
        {
            base.Start();

            _patientIdentifierType = ClearCanvas.Healthcare.PatientIdentifierType.MR;

            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string PatientIdentifier
        {
            get { return _patientIdentifier; }
            set
            {
                _patientIdentifier = value;
                UpdateDisplay();
            }
        }

        public string PatientIdentifierType
        {
            get { return _patientAdminService.PatientIdentifierTypeEnumTable[_patientIdentifierType].Value; }
            set { _patientIdentifierType = _patientAdminService.PatientIdentifierTypeEnumTable[value].Code; }
        }

        public string[] PatientIdentifierTypeChoices
        {
            get { return _patientAdminService.PatientIdentifierTypeEnumTable.Values; }
        }

        public string FamilyName
        {
            get { return _familyName; }
            set
            {
                _familyName = value;
                UpdateDisplay();
            }
        }

        public string GivenName
        {
            get { return _givenName; }
            set
            {
                _givenName = value;
                UpdateDisplay();
            }
        }

        public string Sex
        {
            get { return _sex == null ? "(Any)" : _patientAdminService.SexEnumTable[(Sex)_sex].Value; }
            set { _sex = value == "(Any)" ? null : (Sex?)_patientAdminService.SexEnumTable[value].Code; }
        }

        public string[] SexChoices
        {
            get
            {
                List<string> values = new List<string>(_patientAdminService.SexEnumTable.Values);
                values.Add("(Any)");
                return values.ToArray();
            }
        }

        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
        }

        public bool SearchEnabled
        {
            get { return _searchEnabled; }
            protected set
            {
                if (_searchEnabled != value)
                {
                    _searchEnabled = value;
                    EventsHelper.Fire(_searchEnabledChanged, this, new EventArgs());
                }
            }
        }

        public event EventHandler SearchEnabledChanged
        {
            add { _searchEnabledChanged += value; }
            remove { _searchEnabledChanged -= value; }
        }

        public void Search()
        {
            if (_patientAdminWorkspace == null)
            {
                // create the workspace if it doesn't exist
                _patientAdminComponent = new PatientAdminComponent();
                _patientAdminWorkspace = ApplicationComponent.LaunchAsWorkspace(
                    this.Host.DesktopWindow,
                    _patientAdminComponent,
                    "Patient Search Results",
                    ResultsWorkspaceClosed);
            }
            else
            {
                // otherwise set it active
                _patientAdminWorkspace.Activate();
            }

            _patientAdminComponent.SetSearchCriteria(BuildCriteria());
        }

        #endregion

        private void UpdateDisplay()
        {
            // ensure the criteria is specific enough before enabling search
            // (eg. sex alone is not specific enough)
            this.SearchEnabled = _patientIdentifier != null || _familyName != null || _givenName != null || _dateOfBirth != null;
        }

        private PatientProfileSearchCriteria BuildCriteria()
        {
            PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
            if (_patientIdentifier != null)
            {
                criteria.Identifiers.Id.Like(_patientIdentifier + "%");
                criteria.Identifiers.Type.EqualTo(_patientIdentifierType);
            }

            if (_familyName != null)
                criteria.Name.FamilyName.Like(_familyName + "%");

            if (_givenName != null)
                criteria.Name.GivenName.Like(_givenName + "%");

            if (_sex != null)
                criteria.Sex.EqualTo((Sex)_sex);

            if (_dateOfBirth != null)
            {
                DateTime start = ((DateTime)_dateOfBirth).Date;
                DateTime end = start + new TimeSpan(23, 59, 59);
                criteria.DateOfBirth.Between(start, end);
            }
            
            return criteria;
        }

        private void ResultsWorkspaceClosed(IApplicationComponent component)
        {
            _patientAdminWorkspace = null;
            _patientAdminComponent = null;
        }


    }
}
