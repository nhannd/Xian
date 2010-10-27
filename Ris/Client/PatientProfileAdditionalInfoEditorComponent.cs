#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Client
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

        private readonly List<EnumValueInfo> _religionChoices;
        private readonly List<EnumValueInfo> _languageChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileAdditionalInfoEditorComponent(List<EnumValueInfo> religionChoices, List<EnumValueInfo> languageChoices)
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
            base.Stop();
        }

        public PatientProfileDetail Subject
        {
            get { return _profile; }
            set { _profile = value; }
        }

        #region Presentation Model

        public EnumValueInfo Religion
        {
            get { return _profile.Religion; }
            set
            {
                _profile.Religion = value;
                this.Modified = true;
            }
        }

        public IList ReligionChoices
        {
            get { return _religionChoices; }
        }

        public EnumValueInfo Language
        {
            get { return _profile.PrimaryLanguage; }
            set
            {
                _profile.PrimaryLanguage = value;
                this.Modified = true;
            }
        }

        public IList LanguageChoices
        {
            get { return _languageChoices; }
        }

        #endregion
    }
}