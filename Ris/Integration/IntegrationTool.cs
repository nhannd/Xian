using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.StudyFinders.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Integration
{
    [MenuAction("apply", "global-menus/Tools/Integration")]
    [ButtonAction("apply", "global-toolbars/Tools/Integration")]
    [Tooltip("apply", "Integration")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class IntegrationTool : Tool<IDesktopToolContext>
    {
        public void Apply()
        {
            BackgroundTask task = new BackgroundTask(IntegrationMethod, false);

            try
            {
                ProgressDialog.Show(task, this.Context.DesktopWindow, true);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }

        private void IntegrationMethod(IBackgroundTaskContext context)
        {
            try
            {
                context.ReportProgress(new BackgroundTaskProgress(0, "Find all studies in viewer database..."));
                StudyItemList studies = GetAllStudies();

                int step = 0;
                int studyNumber = 0;
                int totalStep = 6 * studies.Count;

                foreach (StudyItem study in studies)
                {
                    studyNumber++;

                    string commonMessage = String.Format("Study #{0}: {1} {2}\r\n", studyNumber, study.PatientsName.FirstName, study.PatientsName.LastName);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Find diagnostic service name with study description..."));
                    string diagnosticServiceName = GetDiagnosticServiceName(study.StudyDescription);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate an anonymized patient in Ris database..."));
                    PatientProfileDetail profileDetail = GeneratePatientProfile();
                    EntityRef patientRef = null;
                    EntityRef profileRef = null;
                    AddPatientProfile(profileDetail, out patientRef, out profileRef);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate a visit..."));
                    VisitSummary visit = GenerateVisit(patientRef, profileDetail.Mrn.AssigningAuthority);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate an order..."));
                    OrderDetail newOrder = GenerateRandomOrder(profileRef, diagnosticServiceName, visit);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Duplicating study with anonymized patient data..."));
                    List<string> filePaths = CreateNewStudy(study, profileDetail, newOrder);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Importing new study..."));
                    ImportFiles(filePaths);
                }
            }
            catch (Exception e)
            {
                context.Error(e);
            }
        }

        private int CalculatePercentage(int step, int totalStep)
        {
            decimal result = (decimal)step / totalStep * 100;
            return (int)Math.Floor(result);
        }

        private string GetRandomName(string fileName)
        {
            List<string> names = new List<string>();

            using (StreamReader reader = File.OpenText(fileName))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    names.Add(line);
                }
            }

            return RandomUtils.ChooseRandom<string>(names);
        }

        #region Ris Related Functions

        private string GetDiagnosticServiceName(string studyDescription)
        {
            string diagnosticServiceName = "";

            // Using the study description, look for a corresponding diagnostic service name in a CSV mapping file
            using (StreamReader reader = File.OpenText(IntegrationSettings.Default.MappingFile))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] row = line.Split(new string[] { "," }, StringSplitOptions.None);
                    if (String.Compare(row[0], studyDescription) == 0)
                    {
                        diagnosticServiceName = row[1];
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(diagnosticServiceName))
                throw new Exception(String.Format(SR.MessageFailedToFindDiagnosticServiceName, studyDescription, IntegrationSettings.Default.MappingFile));

            return diagnosticServiceName;
        }

        private PatientProfileDetail GeneratePatientProfile()
        {
            PatientProfileDetail profile = null;

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    LoadPatientProfileEditorFormDataResponse response = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());

                    DateTime now = Platform.Time;

                    profile = new PatientProfileDetail();
                    profile.Mrn = new MrnDetail(DateTimeFormat.Format(now, "YYYYMMDDhhmmssxxx"),
                        RandomUtils.ChooseRandom<string>(response.MrnAssigningAuthorityChoices));
                    profile.Healthcard = new HealthcardDetail(
                        RandomUtils.GenerateRandomIntegerString(10),
                        RandomUtils.ChooseRandom<string>(response.HealthcardAssigningAuthorityChoices),
                        "", null);

                    profile.DateOfBirth = now;
                    profile.Sex = RandomUtils.ChooseRandom<EnumValueInfo>(response.SexChoices);
                    profile.PrimaryLanguage = RandomUtils.ChooseRandom<EnumValueInfo>(response.PrimaryLanguageChoices);
                    profile.Religion = RandomUtils.ChooseRandom<EnumValueInfo>(response.ReligionChoices);
                    profile.DeathIndicator = false;
                    profile.TimeOfDeath = null;

                    string givenName = "";
                    string familyName = GetRandomName(IntegrationSettings.Default.FamilyNameDictionary);
                    if (profile.Sex.Code == "F")
                        givenName = GetRandomName(IntegrationSettings.Default.FemaleNameDictionary);
                    else
                        givenName = GetRandomName(IntegrationSettings.Default.MaleNameDictionary);

                    givenName += " Anonymous";
                    profile.Name = new PersonNameDetail();
                    profile.Name.FamilyName = familyName;
                    profile.Name.GivenName = givenName;
                });

            return profile;
        }

        private void AddPatientProfile(PatientProfileDetail profileDetail, out EntityRef patientRef, out EntityRef patientProfileRef)
        {
            AdminAddPatientProfileResponse response = null;

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    response = service.AdminAddPatientProfile(new AdminAddPatientProfileRequest(profileDetail));
                });

            patientRef = response.PatientRef;
            patientProfileRef = response.PatientProfileRef;
        }

        private RegistrationWorklistItem GetRandomPatientProfile()
        {
            RegistrationWorklistItem item = null;

            // Look for a patient with the given name that start with a random generated charactor
            // Repeat if no patient is found
            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    PatientProfileSearchData searchData = new PatientProfileSearchData();
                    while (item == null)
                    {
                        searchData.GivenName = RandomUtils.RandomAlphabet.ToString();

                        SearchPatientResponse response = service.SearchPatient(new SearchPatientRequest(searchData));
                        if (response.WorklistItems != null && response.WorklistItems.Count > 0)
                        {
                            item = RandomUtils.ChooseRandom<RegistrationWorklistItem>(response.WorklistItems);
                        }
                    }
                });

            return item;
        }

        private VisitSummary GenerateVisit(EntityRef patientRef, string assigningAuthority)
        {
            VisitSummary visit = null;

            // Generate an active visit with randomize properties
            Platform.GetService<IVisitAdminService>(
                delegate(IVisitAdminService service)
                {
                    LoadVisitEditorFormDataResponse visitFormResponse = service.LoadVisitEditorFormData(new LoadVisitEditorFormDataRequest());

                    DateTime now = Platform.Time;

                    VisitDetail visitDetail = new VisitDetail();
                    visitDetail.Patient = patientRef;
                    visitDetail.VisitNumberId = DateTimeFormat.Format(now, "YYYYMMDDmmssxxx");
                    visitDetail.VisitNumberAssigningAuthority = assigningAuthority;
                    visitDetail.PatientClass = RandomUtils.ChooseRandom<EnumValueInfo>(visitFormResponse.PatientClassChoices);
                    visitDetail.PatientType = RandomUtils.ChooseRandom<EnumValueInfo>(visitFormResponse.PatientTypeChoices);
                    visitDetail.AdmissionType = RandomUtils.ChooseRandom<EnumValueInfo>(visitFormResponse.AdmissionTypeChoices);
                    visitDetail.Status = CollectionUtils.SelectFirst<EnumValueInfo>(visitFormResponse.VisitStatusChoices,
                        delegate(EnumValueInfo enumValue)
                        {
                            return enumValue.Code == "AA";
                        });
                    visitDetail.AdmitDateTime = now;
                    visitDetail.Facility = RandomUtils.ChooseRandom<FacilitySummary>(visitFormResponse.FacilityChoices);

                    AdminAddVisitResponse addVisitResponse = service.AdminAddVisit(new AdminAddVisitRequest(visitDetail));
                    visit = addVisitResponse.AddedVisit;
                });

            return visit;
        }

        private OrderDetail GenerateRandomOrder(EntityRef patientProfileRef, string diagnosticServiceName, VisitSummary visit)
        {
            EntityRef orderRef = null;
            OrderDetail orderDetail = null;

            // Generate an order with randomize properties
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    GetOrderEntryFormDataResponse formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest());

                    DiagnosticServiceSummary mappedDS = CollectionUtils.SelectFirst<DiagnosticServiceSummary>(
                        formChoicesResponse.DiagnosticServiceChoices,
                        delegate(DiagnosticServiceSummary ds)
                        {
                            return ds.Name == diagnosticServiceName;
                        });

                    if (mappedDS == null)
                        throw new Exception(String.Format("Cannot find diagnostic service with name {0}", diagnosticServiceName));

                    FacilitySummary randomFacility = RandomUtils.ChooseRandom<FacilitySummary>(formChoicesResponse.OrderingFacilityChoices);
                    StaffSummary randomPhysician = RandomUtils.ChooseRandom<StaffSummary>(formChoicesResponse.OrderingPhysicianChoices);
                    EnumValueInfo randomPriority = RandomUtils.ChooseRandom<EnumValueInfo>(formChoicesResponse.OrderPriorityChoices);

                    PlaceOrderResponse response = service.PlaceOrder(new PlaceOrderRequest(
                        visit.Patient,
                        visit.entityRef,
                        mappedDS.DiagnosticServiceRef,
                        randomPriority,
                        randomPhysician.StaffRef,
                        randomFacility.FacilityRef,
                        true,
                        Platform.Time));


                    LoadOrderDetailResponse loadOrderResponse = service.LoadOrderDetail(new LoadOrderDetailRequest(response.OrderRef));
                    orderRef = response.OrderRef;
                    orderDetail = loadOrderResponse.OrderDetail;
                });

            // Completing the modality procedure step of the new order
            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                {
                    ClearCanvas.Ris.Application.Common.ModalityWorkflow.GetWorklistResponse workflowResponse = 
                        service.GetWorklist(new ClearCanvas.Ris.Application.Common.ModalityWorkflow.GetWorklistRequest("ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled"));

                    List<ModalityWorklistItem> listItem = CollectionUtils.Select<ModalityWorklistItem, List<ModalityWorklistItem>>(
                        workflowResponse.WorklistItems,
                        delegate(ModalityWorklistItem item)
                        {
                            return item.AccessionNumber == orderDetail.AccessionNumber;
                        });

                    CollectionUtils.ForEach<ModalityWorklistItem>(listItem,
                        new Action<ModalityWorklistItem>(delegate(ModalityWorklistItem item)
                        {
                            service.CompleteProcedure(new CompleteProcedureRequest(item.ProcedureStepRef));
                        }));
                });

            return orderDetail;
        }

        #endregion

        #region Viewer Related Functions

        private StudyItemList GetAllStudies()
        {
            LocalDataStoreStudyFinder studyFinder = new LocalDataStoreStudyFinder();

            // do a broad query and choose a random Study
            QueryParameters queryParams = new QueryParameters();
            queryParams.Add("PatientsName", "");
            queryParams.Add("PatientId", "");
            queryParams.Add("AccessionNumber", "");
            queryParams.Add("StudyDescription", "");
            queryParams.Add("ModalitiesInStudy", "");
            queryParams.Add("StudyDate", "");
            queryParams.Add("StudyInstanceUid", "");

            return studyFinder.Query(queryParams, null);
        }

        private List<string> CreateNewStudy(StudyItem studyItem, PatientProfileDetail profile, OrderDetail order)
        {
            // Code copied from StudyLoader
            List<string> filePaths = new List<string>();

            IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid(studyItem.StudyInstanceUID));
            IEnumerator<ISopInstance> sops = study.GetSopInstances().GetEnumerator();
            sops.Reset();

            string newStudyInstanceUid = DicomUid.GenerateUid().UID;
            Dictionary<int, string> seriesInstanceUids = new Dictionary<int,string>();

            // iterate through all the sop instances, create new DicomFile with new patient info
            while (sops.MoveNext())
            {
                ImageSopInstance thisSop = sops.Current as ImageSopInstance;
                if (thisSop != null)
                {
                    string newSeriesInstanceUid = "";
                    if (seriesInstanceUids.ContainsKey(thisSop.Series.SeriesNumber) == false)
                        seriesInstanceUids[thisSop.Series.SeriesNumber] = DicomUid.GenerateUid().UID;

                    newSeriesInstanceUid = seriesInstanceUids[thisSop.Series.SeriesNumber];
                    string newSopInstanceUid = DicomUid.GenerateUid().UID;

                    // Change a new Dicom file
                    DicomFile dicomFile = new DicomFile(thisSop.LocationUri.LocalDiskPath);
                    dicomFile.Load(DicomReadOptions.Default);
                    dicomFile.DataSet[DicomTags.StudyInstanceUID].SetStringValue(newStudyInstanceUid);
                    dicomFile.DataSet[DicomTags.SeriesInstanceUID].SetStringValue(newSeriesInstanceUid);
                    dicomFile.DataSet[DicomTags.SOPInstanceUID].SetStringValue(newSopInstanceUid);
                    dicomFile.DataSet[DicomTags.PatientsName].SetStringValue(String.Format("{0}^{1}", profile.Name.FamilyName, profile.Name.GivenName));
                    dicomFile.DataSet[DicomTags.PatientID].SetStringValue(String.Format("{0}{1}", profile.Mrn.AssigningAuthority, profile.Mrn.Id));
                    dicomFile.DataSet[DicomTags.PatientsSex].SetStringValue(profile.Sex.Code);
                    dicomFile.DataSet[DicomTags.AccessionNumber].SetStringValue(order.AccessionNumber);
                    dicomFile.DataSet[DicomTags.PatientsBirthDate].SetStringValue(DateTimeFormat.Format(profile.DateOfBirth.Value, "YYYYMMDD"));
                    dicomFile.Filename = String.Format("{0}\\{1}.dcm", Directory.GetCurrentDirectory(), newSopInstanceUid);
                    dicomFile.Save(DicomWriteOptions.Default);

                    filePaths.Add(dicomFile.Filename);
                }
            }

            return filePaths;
        }

        private void ImportFiles(List<string> filePaths)
        {
            // Code copied from the Import Tool
            FileImportRequest request = new FileImportRequest();
            request.FilePaths = filePaths;
            request.BadFileBehaviour = BadFileBehaviour.Ignore;
            request.Recursive = true;
            request.FileImportBehaviour = FileImportBehaviour.Move;

            LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
            try
            {
                client.Open();
                client.Import(request);
                client.Close();
            }
            catch (EndpointNotFoundException)
            {
                client.Abort();
                throw new Exception(SR.MessageImportLocalDataStoreServiceNotRunning);
            }
            catch (Exception e)
            {
                client.Abort();
                throw new Exception(SR.MessageFailedToImportSelection, e);
            }
        }

        #endregion
    }
}
