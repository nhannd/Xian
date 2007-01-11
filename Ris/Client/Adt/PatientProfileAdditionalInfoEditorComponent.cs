using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientProfileAdditionalInfoEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientProfileAdditionalInfoEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PatientProfileAdditionalInfoEditorComponent class
    /// </summary>
    [AssociateView(typeof(PatientProfileAdditionalInfoEditorComponentViewExtensionPoint))]
    public class PatientProfileAdditionalInfoEditorComponent : ApplicationComponent
    {
        private PatientProfile _patient;

        private IAdtService _adtService;
        private ReligionEnumTable _religionTypes;
        private SpokenLanguageEnumTable _languageTypes;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileAdditionalInfoEditorComponent()
        {
        }

        public override void Start()
        {
            base.Start();

            _adtService = ApplicationContext.GetService<IAdtService>();
            _religionTypes = _adtService.GetReligionEnumTable();
            _languageTypes = _adtService.GetSpokenLanguageEnumTable();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public PatientProfile Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }

        #region Presentation Model

        public string Religion
        {
            get { return _religionTypes[_patient.Religion].Value; }
            set
            {
                _patient.Religion = _religionTypes[value].Code;
                this.Modified = true;
            }
        }

        public string[] ReligionChoices
        {
            get { return _religionTypes.Values; }
        }

        public string Language
        {
            get { return _languageTypes[_patient.PrimaryLanguage].Value; }
            set
            {
                _patient.PrimaryLanguage = _languageTypes[value].Code;
                this.Modified = true;
            }
        }

        public string[] LanguageChoices
        {
            get { return _languageTypes.Values; }
        }

        #endregion
    }
}
