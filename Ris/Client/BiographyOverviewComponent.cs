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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
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
                get { return _component._patientProfile;  }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private readonly EntityRef _patientRef;
        private readonly EntityRef _profileRef;
        private PatientProfileDetail _patientProfile;

        private ToolSet _toolSet;
        private readonly ResourceResolver _resourceResolver;

        private ChildComponentHost _bannerComponentHost;
        private ChildComponentHost _contentComponentHost;

		private EntityRef _selectedOrderRef;
    	private TabComponentContainer _pagesContainer;
		private BiographyOrderHistoryComponent _orderHistoryComponent;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOverviewComponent(
            EntityRef patientRef,
            EntityRef profileRef)
        {
            _patientRef = patientRef;
            _profileRef = profileRef;

            _resourceResolver = new ResourceResolver(this.GetType().Assembly);
        }

        public override void Start()
        {
            // query for the minimum possible amount of patient profile detail, just enough to populate the title
            Platform.GetService<IBrowsePatientDataService>(
                delegate(IBrowsePatientDataService service)
                {
                    GetDataRequest request = new GetDataRequest();
                    request.GetPatientProfileDetailRequest = new GetPatientProfileDetailRequest();
                    request.GetPatientProfileDetailRequest.PatientProfileRef = _profileRef;

                    // include notes for the notes component
                    request.GetPatientProfileDetailRequest.IncludeNotes = true;

                    // include attachments for the docs component
                    request.GetPatientProfileDetailRequest.IncludeAttachments = true;
                    GetDataResponse response = service.GetData(request);

                    _patientProfile = response.GetPatientProfileDetailResponse.PatientProfile;
                });

            this.Host.Title = string.Format(SR.TitleBiography,
                    PersonNameFormat.Format(_patientProfile.Name),
                    MrnFormat.Format(_patientProfile.Mrn));

            // Create component for each tab
            _orderHistoryComponent = new BiographyOrderHistoryComponent(_patientRef);
            BiographyNoteComponent noteComponent = new BiographyNoteComponent(_patientProfile.Notes);
            BiographyDemographicComponent demographicComponent = new BiographyDemographicComponent(_patientRef, _profileRef);
            AttachedDocumentPreviewComponent documentComponent = new AttachedDocumentPreviewComponent();
			BiographyVisitHistoryComponent visitHistoryComponent = new BiographyVisitHistoryComponent(_patientRef);
            documentComponent.PatientAttachments = _patientProfile.Attachments;

            // Create tab and tab groups
			_pagesContainer = new TabComponentContainer();
			_pagesContainer.Pages.Add(new TabPage(SR.TitleOrders, _orderHistoryComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitleVisits, visitHistoryComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitleDemographicProfiles, demographicComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitlePatientAttachments, documentComponent));
			_pagesContainer.Pages.Add(new TabPage(SR.TitlePatientNotes, noteComponent));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
			tabGroupContainer.AddTabGroup(new TabGroup(_pagesContainer, 1.0f));

            _contentComponentHost = new ChildComponentHost(this.Host, tabGroupContainer);
            _contentComponentHost.StartComponent();

            _bannerComponentHost = new ChildComponentHost(this.Host, new BannerComponent(_patientProfile));
            _bannerComponentHost.StartComponent();

            _toolSet = new ToolSet(new PatientBiographyToolExtensionPoint(), new PatientBiographyToolContext(this));

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

		// Bug 4786: TableView has trouble selecting items other than the first item
		// This method is a work around for the above defect for the BiographyOrderHistoryComponent, where TableView automatically select 
		// the first item and delay posting this selection event after the Start method, leading to the first item always being selected.
		// The OnControlLoad is called by the control during its Load event, which happens after the delayed selection changed event
		// and before the control is visible.  This work around has to be done for each components that wants to override the initial
		// selected item.
		public void OnControlLoad()
		{
			_orderHistoryComponent.SelectedOrderRef = _selectedOrderRef;
		}
		
		public EntityRef SelectedOrderRef
		{
			get { return _selectedOrderRef; }
			set
			{
				_selectedOrderRef = value;
				if (_orderHistoryComponent != null)
				{
					SetOrderTabAsActiveTabPage();
					_orderHistoryComponent.SelectedOrderRef = value;
				}
			}
		}

		private void SetOrderTabAsActiveTabPage()
		{
			TabPage orderTabPage = CollectionUtils.SelectFirst(
				_pagesContainer.Pages,
				delegate(TabPage tabPage) { return tabPage.Component == _orderHistoryComponent; });

			if (orderTabPage != null)
				_pagesContainer.CurrentPage = orderTabPage;
		}
    }
}
