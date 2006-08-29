using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientSearchToolViewExtensionPoint : ExtensionPoint<IToolView>
    {
    }

    [MenuAction("showHide", "Admin/Patient/Find...")]
    [ButtonAction("showHide", "PatientAdminToolbar/Find")]
    [Tooltip("showHide", "Find Patient")]
    [IconSet("showHide", IconScheme.Colour, "Icons.FindPatientMedium.png", "Icons.FindPatientMedium.png", "Icons.FindPatientMedium.png")]
    [ClickHandler("showHide", "ShowHide")]
    [CheckedStateObserver("showHide", "IsViewActive", "ViewActivationChanged")]

    [ToolView(typeof(PatientSearchToolViewExtensionPoint), "Find Patient", ToolViewDisplayHint.DockLeft, "IsViewActive", "ViewActivationChanged")]

    [ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
    public class PatientSearchTool : Tool
    {
        private bool _viewActive;
        private event EventHandler _viewActivationChanged;

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

        public override void Initialize()
        {
            base.Initialize();

            _patientIdentifierType = ClearCanvas.Healthcare.PatientIdentifierType.MR;

            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public bool IsViewActive
        {
            get { return _viewActive; }
            set
            {
                if (value != _viewActive)
                {
                    _viewActive = value;
                    EventsHelper.Fire(_viewActivationChanged, this, new EventArgs());
                }
            }
        }

        public event EventHandler ViewActivationChanged
        {
            add { _viewActivationChanged += value; }
            remove { _viewActivationChanged -= value; }
        }

        public void ShowHide()
        {
            this.IsViewActive = !this.IsViewActive;
        }

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
                    _patientAdminComponent,
                    "Patient Search Results",
                    ResultsWorkspaceClosed);
            }
            else
            {
                // otherwise set it active
                _patientAdminWorkspace.IsActivated = true;
            }

            _patientAdminComponent.SetSearchCriteria(BuildCriteria());
        }

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
