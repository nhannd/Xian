#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

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
			private PatientProfileSummary _patientProfile;

			public PatientProfileSummary PatientProfile
			{
				get { return _patientProfile; }
				set
				{
					_patientProfile = value;
					this.SetUrl(WebResourcesSettings.Default.BiographyPatientProfilePageUrl);
				}
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return PatientProfile;
			}
		}


		private readonly EntityRef _defaultProfileRef;
		private readonly EntityRef _patientRef;

		private PatientProfileSummary _selectedProfile;
		private List<PatientProfileSummary> _profileChoices;

		private ProfileViewComponent _profileViewComponent;
		private ChildComponentHost _profileViewComponentHost;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyDemographicComponent(EntityRef patientRef, EntityRef defaultProfileRef)
		{
			Platform.CheckForNullReference(patientRef, "patientRef");
			_patientRef = patientRef;

			// default profile ref may be null
			_defaultProfileRef = defaultProfileRef;

			_profileChoices = new List<PatientProfileSummary>();
		}

		public override void Start()
		{
			_profileViewComponent = new ProfileViewComponent();
			_profileViewComponentHost = new ChildComponentHost(this.Host, _profileViewComponent);
			_profileViewComponentHost.StartComponent();

			LoadPatientProfile();

			base.Start();
		}

		public override void Stop()
		{
			if (_profileViewComponentHost != null)
			{
				_profileViewComponentHost.StopComponent();
				_profileViewComponentHost = null;
			}

			base.Stop();
		}

		public string FormatPatientProfile(object item)
		{
			var summary = (PatientProfileSummary)item;
			return String.Format("{0} - {1}", MrnFormat.Format(summary.Mrn), PersonNameFormat.Format(summary.Name));
		}

		#region Presentation Model

		public ApplicationComponentHost ProfileViewComponentHost
		{
			get { return _profileViewComponentHost; }
		}

		public IList ProfileChoices
		{
			get { return _profileChoices; }
		}

		public PatientProfileSummary SelectedProfile
		{
			get { return _selectedProfile; }
			set
			{
				if (!Equals(value, _selectedProfile))
				{
					_selectedProfile = value;
					((ProfileViewComponent)_profileViewComponentHost.Component).PatientProfile = _selectedProfile;
				}
			}
		}

		#endregion

		private void LoadPatientProfile()
		{
			Async.CancelPending(this);

			if (_patientRef == null)
				return;

			Async.Request(this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
						{
							ListPatientProfilesRequest = new ListPatientProfilesRequest(_patientRef)
						};
					return service.GetData(request);
				},
				response =>
				{
					_profileChoices = response.ListPatientProfilesResponse.Profiles;

					_selectedProfile = _defaultProfileRef != null 
						? CollectionUtils.SelectFirst(_profileChoices, pp => pp.PatientProfileRef.Equals(_defaultProfileRef, true)) 
						: CollectionUtils.FirstElement(_profileChoices);

					_profileViewComponent.PatientProfile = _selectedProfile;
				});
		}
		
	}
}
