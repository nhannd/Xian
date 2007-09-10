using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientBiography;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PatientOverviewDocument : Document
    {
        private EntityRef _profileRef;
        private PatientProfileDetail _patientProfile;

        public PatientOverviewDocument(EntityRef profileRef, IDesktopWindow window)
            :base(profileRef, window)
        {
            _profileRef = profileRef;
        }

        protected override string GetTitle()
        {
            if (_patientProfile != null)
            {
                return String.Format(SR.TitlePatientComponent,
                    PersonNameFormat.Format(_patientProfile.Name),
                    MrnFormat.Format(_patientProfile.Mrn));
            }

            return SR.TitlePatientProfile;   // doesn't matter, cause the component will set the title when it starts
        }

        protected override IApplicationComponent GetComponent()
        {
            List<AlertNotificationDetail> alertNotifications = null;

            Platform.GetService<IPatientBiographyService>(
                delegate(IPatientBiographyService service)
                {
                    LoadPatientProfileResponse response = service.LoadPatientProfile(new LoadPatientProfileRequest(_profileRef));
                    _profileRef = response.PatientProfileRef;
                    _patientProfile = response.PatientDetail;
                    alertNotifications = response.AlertNotifications;
                });
            
            // Create component for each tab
            BiographyOrderHistoryComponent orderHistoryComponent = new BiographyOrderHistoryComponent(_profileRef);
            BiographyDocumentComponent documentComponent = new BiographyDocumentComponent();
            BiographyNoteComponent noteComponent = new BiographyNoteComponent(_patientProfile.Notes);
            BiographyFeedbackComponent feedbackComponent = new BiographyFeedbackComponent();
            BiographyDemographicComponent demographicComponent = new BiographyDemographicComponent(_profileRef, _patientProfile);

            // Create tab and tab groups
            TabComponentContainer tabContainer = new TabComponentContainer();
            tabContainer.Pages.Add(new TabPage(SR.TitleOrders, orderHistoryComponent));
            tabContainer.Pages.Add(new TabPage(SR.TitleDemographic, demographicComponent));
            tabContainer.Pages.Add(new TabPage(SR.TitleDocuments, documentComponent));
            tabContainer.Pages.Add(new TabPage(SR.TitleNotes, noteComponent));
            tabContainer.Pages.Add(new TabPage(SR.TitlePatientFeedbacks, feedbackComponent));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer, 1.0f));

            // Construct the Patient Biography page
            return new SplitComponentContainer(
                new SplitPane("", new PatientOverviewComponent(_profileRef, _patientProfile, alertNotifications), true),
                new SplitPane("", tabGroupContainer, 0.8f),
                SplitOrientation.Horizontal);
        }
    }    
}
