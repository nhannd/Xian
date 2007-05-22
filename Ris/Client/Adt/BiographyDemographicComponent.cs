using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="BiographyDemographicComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class BiographyDemographicComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// BiographyDemographicComponent class
    /// </summary>
    [AssociateView(typeof(BiographyDemographicComponentViewExtensionPoint))]
    public class BiographyDemographicComponent : ApplicationComponent
    {
        private PatientProfileDetail _patientProfile;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyDemographicComponent(PatientProfileDetail patientProfile)
        {
            _patientProfile = patientProfile;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public string FamilyName
        {
            get { return _patientProfile.Name.FamilyName; }
        }

        public string GivenName
        {
            get { return _patientProfile.Name.GivenName; }
        }

        public string MiddleName
        {
            get { return _patientProfile.Name.MiddleName; }
        }

        public string Prefix
        {
            get { return _patientProfile.Name.Prefix; }
        }

        public string Suffix
        {
            get { return _patientProfile.Name.Suffix; }
        }

        public string Degree
        {
            get { return _patientProfile.Name.Degree; }
        }

        public string Sex
        {
            get { return _patientProfile.Sex.Value; }
        }

        public string DateOfBirth
        {
            get { return Format.Date(_patientProfile.DateOfBirth); }
        }

        public string TimeOfDeath
        {
            get { return Format.DateTime(_patientProfile.TimeOfDeath); }
        }

        public string Religion
        {
            get { return _patientProfile.Religion.Value; }
        }

        public string PrimaryLanguage
        {
            get { return _patientProfile.PrimaryLanguage.Value; }
        }

        public string Mrn
        {
            get { return _patientProfile.Mrn.Id; }
        }

        public string MrnSite
        {
            get { return _patientProfile.Mrn.AssigningAuthority; }
        }

        public string Healthcard
        {
            get { return _patientProfile.Healthcard.Id; }
        }

        public string HealthcardProvince
        {
            get { return _patientProfile.Healthcard.AssigningAuthority; }
        }

        public string HealthcardVersionCode
        {
            get { return _patientProfile.Healthcard.VersionCode; }
        }

        public string HealthcardExpiry
        {
            get { return Format.Date(_patientProfile.Healthcard.ExpiryDate); }
        }

        #endregion
    }
}
