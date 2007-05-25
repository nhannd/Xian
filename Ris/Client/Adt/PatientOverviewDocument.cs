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

            try
            {
                Platform.GetService<IPatientBiographyService>(
                    delegate(IPatientBiographyService service)
                    {
                        LoadPatientProfileResponse response = service.LoadPatientProfile(new LoadPatientProfileRequest(_profileRef));
                        _profileRef = response.PatientProfileRef;
                        _patientProfile = response.PatientDetail;
                        alertNotifications = response.AlertNotifications;
                    });

            }
            catch (Exception e)
            {
                // TODO: Report this...
            }
            
            // Create component for each tab
            BiographyOrderHistoryComponent orderHistoryComponent = new BiographyOrderHistoryComponent(_profileRef);
            BiographyDocumentComponent documentComponent = new BiographyDocumentComponent();
            BiographyNoteComponent noteComponent = new BiographyNoteComponent(_patientProfile.Notes);
            BiographyFeedbackComponent feedbackComponent = new BiographyFeedbackComponent();
            BiographyDemographicComponent demographicComponent = new BiographyDemographicComponent(_profileRef, _patientProfile);

            // Create tab and tab groups
            TabComponentContainer tabContainer1 = new TabComponentContainer();
            tabContainer1.Pages.Add(new TabPage(SR.TitleOrders, orderHistoryComponent));

            TabComponentContainer tabContainer2 = new TabComponentContainer();
            tabContainer2.Pages.Add(new TabPage(SR.TitleDemographic, demographicComponent));
            tabContainer2.Pages.Add(new TabPage(SR.TitleDocuments, documentComponent));
            tabContainer2.Pages.Add(new TabPage(SR.TitleNotes, noteComponent));
            tabContainer2.Pages.Add(new TabPage(SR.TitlePatientFeedbacks, feedbackComponent));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer1, 0.5f));
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer2, 0.5f));

            // Construct the Patient Biography page
            return new SplitComponentContainer(
                new SplitPane("", new PatientOverviewComponent(_profileRef, _patientProfile, alertNotifications), true),
                new SplitPane("", tabGroupContainer, 0.8f),
                SplitOrientation.Horizontal);
        }
    }    
}
