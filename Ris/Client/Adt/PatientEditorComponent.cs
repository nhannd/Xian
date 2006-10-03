using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

using Iesi.Collections;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint()]
    public class PatientEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PatientEditorComponentViewExtensionPoint))]
    public class PatientEditorComponent : ApplicationComponent
    {
        private PatientProfile _patient;
        private IPatientAdminService _patientAdminService;

        private string[] _dummyProvinceChoices = new string[] { "Ontario", "Alberta", "British Columbia", "Manitoba", "New Brunswick", "Newfoundland", "Nova Scotia", "PEI", "Quebec", "Saskatchewan" };
        private string[] _dummySiteChoices = new string[] { "UHN", "MSH", "SiteA", "SiteB", "SiteC", "SiteD", "SiteE", "SiteF" };

        public PatientEditorComponent()
        {
            _patient = PatientProfile.New();
        }

        /// <summary>
        /// Gets or sets the subject (e.g PatientProfile) that this editor operates on.  This property
        /// should never be used by the view.
        /// </summary>
        public PatientProfile Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public override void Start()
        {
            base.Start();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public string FamilyName
        {
            get { return _patient.Name.FamilyName; }
            set { 
                _patient.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _patient.Name.GivenName; }
            set { 
                _patient.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _patient.Name.MiddleName; }
            set { 
                _patient.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Sex
        {
            get { return _patientAdminService.SexEnumTable[_patient.Sex].Value; }
            set { 
                _patient.Sex = _patientAdminService.SexEnumTable[value].Code;
                this.Modified = true;
            }
        }

        public string[] SexChoices
        {
            get { return _patientAdminService.SexEnumTable.Values; }
        }

        public DateTime DateOfBirth
        {
            get { return _patient.DateOfBirth; }
            set { 
                _patient.DateOfBirth = new DateTime(value.Year, value.Month, value.Day);
                this.Modified = true;
            }
        }

        public bool DeathIndicator
        {
            get { return _patient.DeathIndicator; }
            set { 
                _patient.DeathIndicator = value;
                this.Modified = true;
            }
        }

        public DateTime? TimeOfDeath
        {
            get { return _patient.TimeOfDeath; }
            set
            {
                _patient.TimeOfDeath = value;
                this.Modified = true;
            }
        }

        public string MrnID
        {
            get { return _patient.MRN.Id; }
            set
            {
                _patient.MRN.Id = value;
                this.Modified = true;
            }
        }

        public string MrnSite
        {
            get { return _patient.MRN.AssigningAuthority; }
            set
            {
                _patient.MRN.AssigningAuthority = value;
                this.Modified = true;
            }
        }

        public string[] MrnSiteChoices
        {
            get { return _dummySiteChoices;  }
        }


        public string HealthcardID
        {
            get { return _patient.Healthcard.Id; }
            set
            {
                _patient.Healthcard.Id = value;
                this.Modified = true;
            }
        }

        public string HealthcardProvince
        {
            get { return _patient.Healthcard.AssigningAuthority; }
            set
            {
                _patient.Healthcard.AssigningAuthority = value;
                this.Modified = true;
            }
        }

        public string[] HealthcardProvinceChoices
        {
            get { return _dummyProvinceChoices;  }
        }

    }
}
