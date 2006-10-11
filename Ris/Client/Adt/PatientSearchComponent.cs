using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common.Utilities;
using System.ComponentModel;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientSearchRequestedEventArgs : EventArgs
    {
        private PatientProfileSearchCriteria _criteria;

        public PatientSearchRequestedEventArgs(PatientProfileSearchCriteria criteria)
	    {
            _criteria = criteria;
	    }

        public PatientProfileSearchCriteria SearchCriteria
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
        private IPatientAdminService _patientAdminService;
        private event EventHandler<PatientSearchRequestedEventArgs> _searchRequested;

        private string _mrn;
        private string _healthcard;
        private string _familyName;
        private string _givenName;
        private Sex? _sex;
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

        [ValidateMatchRegex(@"\d\d\d\d\d\d\d+", "MRN must be 7 or more digits")]
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

        [ValidateMatchRegex(@"\w\w+", "Must provide at least 2 letters of family name")]
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
                if (!_keepOpen)
                {
                    this.Host.Exit();
                }

                // always turn the validation errors off after a successful search
                this.ShowValidationErrors = false;
            }
            else
            {
                this.ShowValidationErrors = true;
            }
        }

        #endregion

        private void UpdateDisplay()
        {
            // ensure the criteria is specific enough before enabling search
            // (eg. sex alone is not specific enough)
            this.SearchEnabled = _mrn != null || _healthcard != null || _familyName != null || _givenName != null;
        }

        private PatientProfileSearchCriteria BuildCriteria()
        {
            PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
            if (_mrn != null)
            {
                criteria.MRN.Id.Like(_mrn + "%");
            }

            if (_healthcard != null)
            {
                criteria.Healthcard.Id.Like(_healthcard + "%");
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
    }
}
