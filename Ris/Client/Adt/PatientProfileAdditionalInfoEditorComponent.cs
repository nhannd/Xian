using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

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
        private PatientProfileDetail _profile;

        private List<EnumValueInfo> _religionChoices;
        private List<EnumValueInfo> _languageChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileAdditionalInfoEditorComponent(IList<EnumValueInfo> religionChoices, IList<EnumValueInfo> languageChoices)
        {
            _languageChoices = languageChoices;
            _religionChoices = religionChoices;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public PatientProfileDetail Subject
        {
            get { return _profile; }
            set { _profile = value; }
        }

        #region Presentation Model

        public string Religion
        {
            get { return _profile.Religion.Value; }
            set
            {
                _profile.Religion = EnumValueUtils.MapDisplayValue(_religionChoices, value);
                this.Modified = true;
            }
        }

        public List<string> ReligionChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_religionChoices); }
        }

        public string Language
        {
            get { return _profile.PrimaryLanguage.Value; }
            set
            {
                _profile.PrimaryLanguage = EnumValueUtils.MapDisplayValue(_languageChoices, value);
                this.Modified = true;
            }
        }

        public List<string> LanguageChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_languageChoices); }
        }

        #endregion
    }
}
