using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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
                    String.Format("{0}, {1}", _patientProfile.Name.FamilyName, _patientProfile.Name.GivenName),
                    String.Format("{0} {1}", _patientProfile.MrnAssigningAuthority, _patientProfile.Mrn));
            }

            return SR.TitlePatientProfile;   // doesn't matter, cause the component will set the title when it starts
        }

        protected override IApplicationComponent GetComponent()
        {
            List<AlertNotificationDetail> alertNotifications = null;

            try
            {
                Platform.GetService<IRegistrationWorkflowService>(
                    delegate(IRegistrationWorkflowService service)
                    {
                        LoadPatientProfileForBiographyResponse response = service.LoadPatientProfileForBiography(new LoadPatientProfileForBiographyRequest(_profileRef));
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
            BiographyOrderHistoryComponent orderHistoryComponent = new BiographyOrderHistoryComponent();

            BiographyDocumentComponent documentComponent = new BiographyDocumentComponent();

            StackTabComponentContainer noteComponent = new StackTabComponentContainer(StackStyle.ShowMultiple);
            noteComponent.Pages.Add(new TabPage("Notes", new BiographyNoteComponent(_patientProfile.Notes)));
            noteComponent.Pages.Add(new TabPage("Patient Comments/Feedback", new BiographyNoteComponent(_patientProfile.Notes)));

            StackTabComponentContainer testStackComponent = new StackTabComponentContainer(StackStyle.ShowMultiple);
            testStackComponent.Pages.Add(new TabPage("Test1", new PatientSearchComponent()));
            testStackComponent.Pages.Add(new TabPage("Test2", new PatientSearchComponent()));
            testStackComponent.Pages.Add(new TabPage("Test3", new PatientSearchComponent()));

            StackTabComponentContainer testStackComponent2 = new StackTabComponentContainer(StackStyle.ShowOneOnly);
            testStackComponent2.Pages.Add(new TabPage("Test1", new PatientSearchComponent()));
            testStackComponent2.Pages.Add(new TabPage("Test2", new PatientSearchComponent()));
            testStackComponent2.Pages.Add(new TabPage("Test3", new PatientSearchComponent()));

            // Create tab and tab groups
            TabComponentContainer tabContainer1 = new TabComponentContainer();
            tabContainer1.Pages.Add(new TabPage("Order History", orderHistoryComponent));

            TabComponentContainer tabContainer2 = new TabComponentContainer();
            tabContainer2.Pages.Add(new TabPage("Documents", documentComponent));
            tabContainer2.Pages.Add(new TabPage("Notes", noteComponent));
            tabContainer2.Pages.Add(new TabPage("Test - Stack1", testStackComponent));
            tabContainer2.Pages.Add(new TabPage("Test - Stack2", testStackComponent2));

            TabGroupComponentContainer tabGroupContainer = new TabGroupComponentContainer(LayoutDirection.Horizontal);
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer1, 0.5f));
            tabGroupContainer.AddTabGroup(new TabGroup(tabContainer2, 0.5f));

            // Construct the Patient Biography page
            return new SplitComponentContainer(
                new SplitPane("Summary", new PatientOverviewComponent(_profileRef, _patientProfile, alertNotifications), true),
                new SplitPane("Pages", tabGroupContainer, 0.8f),
                SplitOrientation.Horizontal);
        }
    }
}
