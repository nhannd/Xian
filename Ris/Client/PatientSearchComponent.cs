using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using System.Collections.Generic;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientSearchComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class PatientSearchToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IPatientSearchToolContext : IToolContext
    {
        event EventHandler SelectedProfileChanged;
        PatientProfileSummary SelectedProfile { get; }
        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// PatientSearchComponent class
    /// </summary>
    [AssociateView(typeof(PatientSearchComponentViewExtensionPoint))]
    public class PatientSearchComponent : ApplicationComponent
    {
        class PatientSearchToolContext : ToolContext, IPatientSearchToolContext
        {
            private readonly PatientSearchComponent _component;

            public PatientSearchToolContext(PatientSearchComponent component)
            {
                _component = component;
            }

            public event EventHandler SelectedProfileChanged
            {
                add { _component.SelectedProfileChanged += value; }
                remove { _component.SelectedProfileChanged -= value; }
            }

            public PatientProfileSummary SelectedProfile
            {
                get { return (PatientProfileSummary)_component.SelectedProfile.Item; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private string _mrnID;
        private string _mrnAssigningAuthority;
        private string _healthcard;
        private string _familyName;
        private string _givenName;
        private EnumValueInfo _sex;
        private DateTime? _dateOfBirth;

        private List<EnumValueInfo> _sexChoices;

        private PatientProfileTable _profileTable;
        private PatientProfileSummary _selectedProfile;
        private event EventHandler _selectedProfileChanged;

        private ToolSet _toolSet;

        public override void Start()
        {
            _profileTable = new PatientProfileTable();
            _toolSet = new ToolSet(new PatientSearchToolExtensionPoint(), new PatientSearchToolContext(this));

            try
            {
                Platform.GetService<IRegistrationWorkflowService>(
                    delegate(IRegistrationWorkflowService service)
                        {
                            LoadSearchPatientFormDataResponse response = service.LoadSearchPatientFormData(new LoadSearchPatientFormDataRequest());
                            _sexChoices = response.SexChoices;
                        });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Presentation Model

        public string MrnID
        {
            get { return _mrnID; }
            set { _mrnID = value; }
        }

        public string MrnAssigningAuthority
        {
            get { return _mrnAssigningAuthority; }
            set { _mrnAssigningAuthority = value; }
        }

        public string Healthcard
        {
            get { return _healthcard; }
            set { _healthcard = value; }
        }

        public string HealthcardMask
        {
            get { return TextFieldMasks.HealthcardNumberMask; }
        }

        public string FamilyName
        {
            get { return _familyName; }
            set { _familyName = value; }
        }

        public string GivenName
        {
            get { return _givenName; }
            set { _givenName = value; }
        }

        public string Sex
        {
            get { return _sex != null ? _sex.Value : null; }
            set
            {
                _sex = EnumValueUtils.MapDisplayValue(_sexChoices, value);
            }
        }

        public DateTime? DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
        }

        public bool SearchEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(_mrnID) ||
                       !String.IsNullOrEmpty(_mrnAssigningAuthority) ||
                       !String.IsNullOrEmpty(_healthcard) ||
                       !String.IsNullOrEmpty(_familyName) ||
                       !String.IsNullOrEmpty(_givenName) ||
                       _sex != null ||
                       _dateOfBirth != null;
            }
        }

        public List<string> SexChoices
        {
            get
            {
                List<string> displayList = EnumValueUtils.GetDisplayValues(_sexChoices);
                displayList.Insert(0, "Any");
                return displayList;
            }
        }

        public ITable Profiles
        {
            get { return _profileTable; }
        }

        public ActionModelRoot ItemsContextMenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-contextmenu", _toolSet.Actions);
            }
        }

        public ActionModelNode ItemsToolbarModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "patientsearch-items-toolbar", _toolSet.Actions);
            }
        }

        public override IActionSet ExportedActions
        {
            get { return _toolSet.Actions; }
        }

        public ISelection SelectedProfile
        {
            get { return new Selection(_selectedProfile); }
            set
            {
                _selectedProfile = (PatientProfileSummary)value.Item;
                EventsHelper.Fire(_selectedProfileChanged, this, EventArgs.Empty);
            }
        }

        public event EventHandler SelectedProfileChanged
        {
            add { _selectedProfileChanged += value; }
            remove { _selectedProfileChanged -= value; }
        }

        public void Search()
        {
            try
            {
                _profileTable.Items.Clear();

                Platform.GetService<IRegistrationWorkflowService>(
                    delegate(IRegistrationWorkflowService service)
                    {
                        SearchPatientResponse response = service.SearchPatient(
                            new SearchPatientRequest(
                            _mrnID,
                            _mrnAssigningAuthority,
                            _healthcard, 
                            _familyName, 
                            _givenName,
                            _sex,
                            _dateOfBirth));

                        _profileTable.Items.AddRange(response.Profiles);
                    });

                this.SelectedProfile = new Selection(_profileTable.Items.Count > 0 ? _profileTable.Items[0] : null);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion
    }
}
