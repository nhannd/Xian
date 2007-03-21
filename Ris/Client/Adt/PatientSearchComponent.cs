using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientSearchRequestedEventArgs : EventArgs
    {
        private PatientProfileSearchData _criteria;

        public PatientSearchRequestedEventArgs(PatientProfileSearchData criteria)
	    {
            _criteria = criteria;
	    }

        public PatientProfileSearchData SearchCriteria
        {
            get { return _criteria; }
        }
    }


    [ExtensionPoint()]
    public class PatientSearchComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PatientSearchComponentViewExtensionPoint))]
    public class PatientSearchComponent : ApplicationComponent
    {
        private event EventHandler<PatientSearchRequestedEventArgs> _searchRequested;

        private string _mrn;
        private string _healthcard;
        private string _familyName;
        private string _givenName;
        private EnumValueInfo _sex;
        private DateTime? _dateOfBirth;
        private bool _keepOpen;

        private SexEnumTable _sexChoices;

        private bool _searchEnabled;
        private event EventHandler _searchEnabledChanged;

        public PatientSearchComponent()
        {
        }

        public override void Start()
        {
            base.Start();

            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
            _sexChoices = _patientAdminService.GetSexEnumTable();
        }

        public override void Stop()
        {
            base.Stop();
        }


        public IDesktopWindow DesktopWindow
        {
            get { return this.Host.DesktopWindow; }
        }

        public PatientProfileSearchCriteria SearchCriteria
        {
            get { return BuildCriteria(); }
        }

        public event EventHandler<PatientSearchRequestedEventArgs> SearchRequested
        {
            add { _searchRequested += value; }
            remove { _searchRequested -= value; }
        }

        #region Presentation Model

        public string Mrn
        {
            get { return _mrn; }
            set
            {
                _mrn = value;

                UpdateDisplay();
            }
        }

        public string Healthcard
        {
            get { return _healthcard; }
            set
            {
                _healthcard = value;

                UpdateDisplay();
            }
        }

        public string HealthcardMask
        {
            get { return TextFieldMasks.HealthcardNumberMask; }
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
            get { return _sex == null ? "(Any)" : _sexChoices[(Sex)_sex].Value; }
            set { _sex = value == "(Any)" ? null : (Sex?)_sexChoices[value].Code; }
        }

        public string[] SexChoices
        {
            get
            {
                List<string> values = new List<string>(_sexChoices.Values);
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

        public bool KeepOpen
        {
            get { return _keepOpen; }
            set { _keepOpen = value; }
        }
		

        public event EventHandler SearchEnabledChanged
        {
            add { _searchEnabledChanged += value; }
            remove { _searchEnabledChanged -= value; }
        }

        public void Search()
        {
            if (!this.HasValidationErrors)
            {
                EventsHelper.Fire(_searchRequested, this, new PatientSearchRequestedEventArgs(BuildCriteria()));

                // always turn the validation errors off after a successful search
                this.ShowValidation(false);

                if (!_keepOpen)
                {
                    this.Host.Exit();
                }

            }
            else
            {
                this.ShowValidation(true);
            }
        }

        #endregion

        private void UpdateDisplay()
        {
            // ensure the criteria is specific enough before enabling search
            // (eg. sex alone is not specific enough)
            this.SearchEnabled = _mrn != null || _healthcard != null || _familyName != null || _givenName != null;
        }

        private PatientProfileSearchData BuildCriteria()
        {
            PatientProfileSearchData criteria = new PatientProfileSearchData();
            if (_mrn != null)
                criteria.MrnID = _mrn;

            if (_healthcard != null)
                criteria.HealthcardID = _healthcard;

            if (_familyName != null)
                criteria.FamilyName = _familyName;

            if (_givenName != null)
                criteria.GivenName = _givenName;

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
    }
}
