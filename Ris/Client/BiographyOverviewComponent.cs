#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyOverviewComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class BiographyOverviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[ExtensionPoint]
	public class PatientBiographyToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IPatientBiographyToolContext : IToolContext
	{
		EntityRef PatientRef { get; }
		EntityRef PatientProfileRef { get; }
		PatientProfileDetail PatientProfile { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	/// <summary>
	/// PatientComponent class
	/// </summary>
	[AssociateView(typeof(BiographyOverviewComponentViewExtensionPoint))]
	public class BiographyOverviewComponent : ApplicationComponent
	{
		class PatientBiographyToolContext : ToolContext, IPatientBiographyToolContext
		{
			private readonly BiographyOverviewComponent _component;

			internal PatientBiographyToolContext(BiographyOverviewComponent component)
			{
				_component = component;
			}

			public EntityRef PatientRef
			{
				get { return _component._patientRef; }
			}

			public EntityRef PatientProfileRef
			{
				get { return _component._profileRef; }
			}

			public PatientProfileDetail PatientProfile
			{
				get { return _component._patientProfile; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}
		}

		private readonly EntityRef _patientRef;
		private readonly EntityRef _profileRef;
		private readonly EntityRef _initialSelectedOrderRef;
		private PatientProfileDetail _patientProfile;

		private ToolSet _toolSet;

		private ChildComponentHost _bannerComponentHost;
		private ChildComponentHost _contentComponentHost;

		private TabComponentContainer _pagesContainer;
		private BannerComponent _bannerComponent;
		private BiographyOrderHistoryComponent _orderHistoryComponent;
		private BiographyVisitHistoryComponent _visitHistoryComponent;
		private BiographyDemographicComponent _demographicComponent;
		private AttachedDocumentPreviewComponent _documentComponent;
		private BiographyNoteComponent _noteComponent;
		private PatientAllergiesComponent _allergyComponent;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyOverviewComponent(EntityRef patientRef, EntityRef profileRef, EntityRef initialSelectedOrderRef)
		{
			_patientRef = patientRef;
			_profileRef = profileRef;
			_initialSelectedOrderRef = initialSelectedOrderRef;
		}

		public override void Start()
		{
			// Create component for each tab
			_bannerComponent = new BannerComponent();
			_orderHistoryComponent = new BiographyOrderHistoryComponent(_initialSelectedOrderRef) { PatientRef = _patientRef };
			_visitHistoryComponent = new BiographyVisitHistoryComponent { PatientRef = _patientRef };
			_demographicComponent = new BiographyDemographicComponent { DefaultProfileRef = _profileRef, PatientRef = _patientRef };
			_documentComponent = new AttachedDocumentPreviewComponent(true, AttachedDocumentPreviewComponent.AttachmentMode.Patient);
			_noteComponent = new BiographyNoteComponent();
			_allergyComponent = new PatientAllergiesComponent();

			// Create tab and tab groups
			_pagesContainer = new TabComponentContainer();
			_pagesContainer.Pages.Add(new TabPage(SR.TitleOrders, _orderHistoryComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitleVisits, _visitHistoryComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitleDemographicProfiles, _demographicComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitlePatientAttachments, _documentComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitlePatientNotes, _noteComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitlePatientAllergies, _allergyComponent));

			var tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
			tabGroupContainer.AddTabGroup(new TabGroup(_pagesContainer, 1.0f));

			_contentComponentHost = new ChildComponentHost(this.Host, tabGroupContainer);
			_contentComponentHost.StartComponent();

			_bannerComponentHost = new ChildComponentHost(this.Host, _bannerComponent);
			_bannerComponentHost.StartComponent();

			_toolSet = new ToolSet(new PatientBiographyToolExtensionPoint(), new PatientBiographyToolContext(this));

			LoadPatientProfile();

			base.Start();
		}

		public override void Stop()
		{
			if (_contentComponentHost != null)
			{
				_contentComponentHost.StopComponent();
				_contentComponentHost = null;
			}

			if (_bannerComponentHost != null)
			{
				_bannerComponentHost.StopComponent();
				_bannerComponentHost = null;
			}

			_toolSet.Dispose();
			base.Stop();
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		#region Presentation Model

		public ApplicationComponentHost BannerComponentHost
		{
			get { return _bannerComponentHost; }
		}

		public ApplicationComponentHost ContentComponentHost
		{
			get { return _contentComponentHost; }
		}

		public int BannerHeight
		{
			get { return BannerSettings.Default.BannerHeight; }
		}

		#endregion

		private void LoadPatientProfile()
		{
			Async.CancelPending(this);

			if (_profileRef == null)
				return;

			Async.Request(this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
						{
							GetPatientProfileDetailRequest = new GetPatientProfileDetailRequest
							{
								PatientProfileRef = _profileRef,
								// include notes for the notes component
								IncludeNotes = true,
								// include attachments for the docs component
								IncludeAttachments = true,
								// include patient allergies for allergies component
								IncludeAllergies = true
							}
						};

					return service.GetData(request);
				},
				response =>
				{
					_patientProfile = response.GetPatientProfileDetailResponse.PatientProfile;

					this.Host.Title = string.Format(SR.TitleBiography, PersonNameFormat.Format(_patientProfile.Name), MrnFormat.Format(_patientProfile.Mrn));
					_bannerComponent.HealthcareContext = _patientProfile;
					_documentComponent.PatientAttachments = _patientProfile.Attachments;
					_noteComponent.Notes = _patientProfile.Notes;
					_allergyComponent.Allergies = _patientProfile.Allergies;

					NotifyPropertyChanged("SelectedOrder");
					NotifyAllPropertiesChanged();
				},
				exception =>
				{
					ExceptionHandler.Report(exception, this.Host.DesktopWindow);

					_patientProfile = null;
					_bannerComponent.HealthcareContext = null;
					_documentComponent.PatientAttachments = new List<PatientAttachmentSummary>();
					_noteComponent.Notes = new List<PatientNoteDetail>();

					NotifyPropertyChanged("SelectedOrder");
					NotifyAllPropertiesChanged();
				});
		}
	}
}
