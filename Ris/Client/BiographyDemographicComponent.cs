#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.Jsml;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Client
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
        class ProfileViewComponent : DHtmlComponent
        {
            [DataContract]
            class Context : DataContractBase
            {
                public Context(EntityRef patientRef, EntityRef profileRef)
                {
                    this.PatientRef = patientRef;
                    this.PatientProfileRef = profileRef;
                }

                [DataMember]
                public EntityRef PatientRef;

                [DataMember]
                public EntityRef PatientProfileRef;

            }

            private BiographyDemographicComponent _owner;

            public ProfileViewComponent(BiographyDemographicComponent owner)
            {
                _owner = owner;
            }

            public override void Start()
            {
                this.SetUrl("http://localhost/RIS/patientprofile.htm");
                base.Start();
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return (_owner._selectedProfile == null) ? null : new Context(_owner._patientRef, _owner._selectedProfile.PatientProfileRef);
            }

        }


        private readonly EntityRef _profileRef;
        private readonly EntityRef _patientRef;
        private PatientProfileDetail _patientProfileDetail;

        private PatientProfileSummary _selectedProfile;
        private List<PatientProfileSummary> _profileChoices;

        private ChildComponentHost _profileViewComponentHost;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyDemographicComponent(EntityRef patientRef, EntityRef profileRef, PatientProfileDetail patientProfile)
        {
            _patientRef = patientRef;
            _profileRef = profileRef;
            _patientProfileDetail = patientProfile;

            _profileChoices = new List<PatientProfileSummary>();
        }

        public override void Start()
        {
            Platform.GetService<IBrowsePatientDataService>(
                delegate(IBrowsePatientDataService service)
                {
                    GetDataRequest request = new GetDataRequest();
                    request.ListPatientProfilesRequest = new ListPatientProfilesRequest(_patientRef);

                    GetDataResponse response = service.GetData(request);
                    _profileChoices = response.ListPatientProfilesResponse.Profiles;
                });

            _selectedProfile = CollectionUtils.FirstElement(_profileChoices);

            _profileViewComponentHost = new ChildComponentHost(this.Host, new ProfileViewComponent(this));
            _profileViewComponentHost.StartComponent();

            base.Start();
        }


        private static string ProfileStringConverter(PersonNameDetail name, CompositeIdentifierDetail mrn)
        {
            return String.Format("{0} - {1}", MrnFormat.Format(mrn), PersonNameFormat.Format(name));
        }

        private void OnSelectedProfileChanged()
        {
            if (_selectedProfile == null)
                return;

            try
            {
                Platform.GetService<IBrowsePatientDataService>(
                    delegate(IBrowsePatientDataService service)
                    {
                        GetDataRequest request = new GetDataRequest();
                        request.GetPatientProfileDetailRequest = new GetPatientProfileDetailRequest(_profileRef, true, true, true, true, true, true, false);
                        GetDataResponse response = service.GetData(request);

                        _patientProfileDetail = response.GetPatientProfileDetailResponse.PatientProfile;
                    });

                NotifyAllPropertiesChanged();       
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        

        #region Presentation Model

        public ApplicationComponentHost ProfileViewComponentHost
        {
            get { return _profileViewComponentHost; }
        }
        
        public List<string> ProfileChoices
        {
            get
            {
                List<string> profileStrings = new List<string>();
                if (_profileChoices.Count > 0)
                {
                    profileStrings.AddRange(
                        CollectionUtils.Map<PatientProfileSummary, string>(
                            _profileChoices, delegate(PatientProfileSummary pp) { return ProfileStringConverter(pp.Name, pp.Mrn); }));
                }

                return profileStrings;
            }
        }

        public string SelectedProfile
        {
            get { return _selectedProfile == null ? "" : ProfileStringConverter(_selectedProfile.Name, _selectedProfile.Mrn); }
            set
            {
                _selectedProfile = (value == "") ? null :
                    CollectionUtils.SelectFirst<PatientProfileSummary>(_profileChoices,
                        delegate(PatientProfileSummary pp) { return ProfileStringConverter(pp.Name, pp.Mrn) == value; });

                OnSelectedProfileChanged();
            }
        }

        #endregion
    }
}
