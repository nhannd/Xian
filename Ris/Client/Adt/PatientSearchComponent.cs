using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientSearchComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientSearchComponent class
    /// </summary>
    [AssociateView(typeof(PatientSearchComponentViewExtensionPoint))]
    public class PatientSearchComponent : ApplicationComponent
    {
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

        private SimpleActionModel _profileActionHandler;

        public override void Start()
        {
            _profileTable = new PatientProfileTable();

            _profileActionHandler = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));


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

        public ActionModelNode ProfileActionModel
        {
            get { return _profileActionHandler; }
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

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        #endregion
    }
}
