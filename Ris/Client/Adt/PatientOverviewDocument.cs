using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
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
            BiographyOrderHistoryComponent orderHistoryComponent = new BiographyOrderHistoryComponent(_profileRef);
            BiographyDocumentComponent documentComponent = new BiographyDocumentComponent();
            BiographyNoteComponent noteComponent = new BiographyNoteComponent(_patientProfile.Notes);
            BiographyFeedbackComponent feedbackComponent = new BiographyFeedbackComponent();
            IApplicationComponent demographicComponent = GetDemographicComponent(_patientProfile);


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

        private IApplicationComponent GetDemographicComponent(PatientProfileDetail patientProfile)
        {
            AddressesSummaryComponent addressesSummary = null;
            PhoneNumbersSummaryComponent phoneNumbersSummary = null;
            EmailAddressesSummaryComponent emailAddressesSummary = null;
            ContactPersonsSummaryComponent contactPersonsSummary = null;

            try
            {
                Platform.GetService<IPatientAdminService>(
                    delegate(IPatientAdminService service)
                    {
                        LoadPatientProfileEditorFormDataResponse response = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());

                        addressesSummary = new AddressesSummaryComponent(response.AddressTypeChoices);
                        phoneNumbersSummary = new PhoneNumbersSummaryComponent(response.PhoneTypeChoices);
                        emailAddressesSummary = new EmailAddressesSummaryComponent();
                        contactPersonsSummary = new ContactPersonsSummaryComponent(response.ContactPersonTypeChoices, response.ContactPersonRelationshipChoices);
                    });
            }
            catch (Exception e)
            {
                // TODO: Report this...
            }

            addressesSummary.Subject = patientProfile.Addresses;
            phoneNumbersSummary.Subject = patientProfile.TelephoneNumbers;
            emailAddressesSummary.Subject = patientProfile.EmailAddresses;
            contactPersonsSummary.Subject = patientProfile.ContactPersons;

            TabComponentContainer demographicCollectionTabContainer = new TabComponentContainer();
            demographicCollectionTabContainer.Pages.Add(new TabPage("Addresses", addressesSummary));
            demographicCollectionTabContainer.Pages.Add(new TabPage("Phone Numbers", phoneNumbersSummary));
            demographicCollectionTabContainer.Pages.Add(new TabPage("Email Addresses", emailAddressesSummary));
            demographicCollectionTabContainer.Pages.Add(new TabPage("Contact Persons", contactPersonsSummary));

            return new SplitComponentContainer(
                new SplitPane("PersonalInfo", new BiographyDemographicComponent(_patientProfile), true),
                new SplitPane("Collections", demographicCollectionTabContainer, 0.8f),
                SplitOrientation.Horizontal);
        }
    }    
}
