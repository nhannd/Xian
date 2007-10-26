#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.IO;
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
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.ImageViewer.StudyFinders.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using GetWorklistRequest = ClearCanvas.Ris.Application.Common.ModalityWorkflow.GetWorklistRequest;
using GetWorklistResponse = ClearCanvas.Ris.Application.Common.ModalityWorkflow.GetWorklistResponse;

namespace ClearCanvas.Ris.Client.Integration
{
    [MenuAction("apply", "global-menus/Tools/Integration")]
    [ButtonAction("apply", "global-toolbars/Tools/Integration")]
    [Tooltip("apply", "Integration")]
    [ClickHandler("apply", "Apply")]
    [IconSet("apply", IconScheme.Colour, "AddToolSmall.png", "AddToolMedium.png", "AddToolLarge.png")]
    [ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.DemoAdmin)]
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

        private static void IntegrationMethod(IBackgroundTaskContext context)
        {
            context.ReportProgress(new BackgroundTaskProgress(0, "Find all studies in viewer database..."));
            StudyItemList studies = GetAllStudies();

            int step = 0;
            int studyIndex = 0;
            int totalStep = 6 * studies.Count;

            List<Exception> failedException = new List<Exception>();
            foreach (StudyItem study in studies)
            {
                try
                {
                    studyIndex++;

                    string commonMessage = String.Format("Study #{0}: {1} {2}\r\n", studyIndex, study.PatientsName.FirstName, study.PatientsName.LastName);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Find diagnostic service name with study description..."));
                    string diagnosticServiceName = GetDiagnosticServiceName(study.StudyDescription);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate an anonymized patient in Ris database..."));
                    PatientProfileDetail profileDetail = GeneratePatientProfile();

                    EntityRef patientRef, profileRef;
                    AddPatientProfile(profileDetail, out patientRef, out profileRef);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate a visit..."));
                    VisitSummary visit = RandomUtils.RandomVisit(patientRef, profileRef, profileDetail.Mrn.AssigningAuthority);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate an order..."));
                    OrderDetail newOrder = GenerateRandomOrder(diagnosticServiceName, visit);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Duplicating study with anonymized patient data..."));
                    List<string> filePaths = CreateNewStudy(study, profileDetail, newOrder);

                    context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Importing new study..."));
                    ImportFiles(filePaths);
                }
                catch (Exception e)
                {
                    string errorMessage = String.Format("Accession#: {0}\r\n{1}", study.AccessionNumber, e.Message);
                    failedException.Add(new Exception(errorMessage, e));
                }
            }

            if (failedException.Count == 1)
            {
                context.Error(failedException[0]);
            }
            else if (failedException.Count > 1)
            {
                string errorMessage = CollectionUtils.Reduce<Exception, string>(failedException, "Failed to perform actions on one or more studies",
                    delegate(Exception e, string memo)
                    {
                        StringBuilder builder = new StringBuilder(memo);
                        if (String.IsNullOrEmpty(memo) == false)
                            builder.AppendLine();

                        builder.Append(e.Message);
                        return builder.ToString();
                    });
                context.Error(new Exception(errorMessage));
            }
        }

        #region Other helper functions

        private static int CalculatePercentage(int step, int totalStep)
        {
            decimal result = (decimal)step / totalStep * 100;
            return (int)Math.Floor(result);
        }

        private static string GetRandomName(string names)
        {
            List<string> nameList = new List<string>();

            using (TextReader reader = new StringReader(names))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    nameList.Add(line);
                }
            }

            return RandomUtils.ChooseRandom(nameList);
        }

        #endregion

        #region Ris Related Functions

        private static string GetDiagnosticServiceName(string studyDescription)
        {
            string diagnosticServiceName = "";

            // Using the study description, look for a corresponding diagnostic service name in a CSV mapping file
            using (TextReader reader = new StringReader(IntegrationSettings.Default.DiagnosticServiceDictionary))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] row = line.Split(new string[] { "," }, StringSplitOptions.None);
                    if (String.Compare(row[0], studyDescription, true) == 0)
                    {
                        diagnosticServiceName = row[1];
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(diagnosticServiceName))
                throw new Exception(String.Format(SR.MessageFailedToFindDiagnosticServiceName, studyDescription));

            return diagnosticServiceName;
        }

        private static PatientProfileDetail GeneratePatientProfile()
        {
            PatientProfileDetail profile = null;

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    LoadPatientProfileEditorFormDataResponse response = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());

                    DateTime now = Platform.Time;

                    profile = new PatientProfileDetail();
                    profile.Mrn = new MrnDetail(RandomUtils.FormatDateTime(now, null),
                        RandomUtils.ChooseRandom(response.MrnAssigningAuthorityChoices));
                    profile.Healthcard = new HealthcardDetail(
                        RandomUtils.GenerateRandomIntegerString(10),
                        RandomUtils.ChooseRandom(response.HealthcardAssigningAuthorityChoices),
                        "", null);

                    profile.DateOfBirth = now;
                    profile.Sex = RandomUtils.ChooseRandom(response.SexChoices);
                    profile.PrimaryLanguage = RandomUtils.ChooseRandom(response.PrimaryLanguageChoices);
                    profile.Religion = RandomUtils.ChooseRandom(response.ReligionChoices);
                    profile.DeathIndicator = false;
                    profile.TimeOfDeath = null;

                    string givenName;
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

        private static void AddPatientProfile(PatientProfileDetail profileDetail, out EntityRef patientRef, out EntityRef profileRef)
        {
            AdminAddPatientProfileResponse response = null;

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    response = service.AdminAddPatientProfile(new AdminAddPatientProfileRequest(profileDetail));
                });

            patientRef = response.PatientRef;
            profileRef = response.PatientProfileRef;
        }

        private static OrderDetail GenerateRandomOrder(string diagnosticServiceName, VisitSummary visit)
        {
            OrderDetail orderDetail = RandomUtils.RandomOrder(visit, diagnosticServiceName);

            // Look for the modality procedure steps of the new order
            List<ModalityWorklistItem> listItem = null;
            Platform.GetService<IModalityWorkflowService>(
                delegate(IModalityWorkflowService service)
                    {
                        GetWorklistResponse workflowResponse = service.GetWorklist(new GetWorklistRequest("ClearCanvas.Healthcare.Workflow.Modality.Worklists+Scheduled"));

                        listItem = CollectionUtils.Select<ModalityWorklistItem, List<ModalityWorklistItem>>(
                            workflowResponse.WorklistItems,
                            delegate(ModalityWorklistItem item)
                            {
                                return item.AccessionNumber == orderDetail.AccessionNumber;
                            });
                    });

            // Completing the modality procedure step of the new order
            Platform.GetService<ITechnologistDocumentationService>(
                delegate(ITechnologistDocumentationService service)
                {
                    CollectionUtils.ForEach<ModalityWorklistItem>(listItem,
                        delegate(ModalityWorklistItem item)
                        {
                            List<EntityRef> mpsRefs = new List<EntityRef>();
                            mpsRefs.Add(item.ProcedureStepRef);
                            StartModalityProcedureStepsResponse startMpsResponse = service.StartModalityProcedureSteps(
                                new StartModalityProcedureStepsRequest(mpsRefs));

                            service.StopModalityPerformedProcedureStep(
                                new StopModalityPerformedProcedureStepRequest(
                                    startMpsResponse.StartedMpps.ModalityPerformendProcedureStepRef,
                                    startMpsResponse.StartedMpps.ExtendedProperties));
                        });

                });

            return orderDetail;
        }

        #endregion

        #region Viewer Related Functions

        private static StudyItemList GetAllStudies()
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

        private static List<string> CreateNewStudy(StudyItem studyItem, PatientProfileDetail profile, OrderDetail order)
        {
            // Code copied from StudyLoader
            List<string> filePaths = new List<string>();

            IStudy study = DataAccessLayer.GetIDataStoreReader().GetStudy(new Uid(studyItem.StudyInstanceUID));
            IEnumerator<ISopInstance> sops = study.GetSopInstances().GetEnumerator();

            string newStudyInstanceUid = DicomUid.GenerateUid().UID;
            Dictionary<int, string> seriesInstanceUids = new Dictionary<int,string>();

            // iterate through all the sop instances, create new DicomFile with new patient info
            while (sops.MoveNext())
            {
                ImageSopInstance thisSop = sops.Current as ImageSopInstance;
                if (thisSop != null)
                {
                    string newSeriesInstanceUid;
                    if (seriesInstanceUids.ContainsKey(thisSop.Series.SeriesNumber) == false)
                        seriesInstanceUids[thisSop.Series.SeriesNumber] = DicomUid.GenerateUid().UID;

                    newSeriesInstanceUid = seriesInstanceUids[thisSop.Series.SeriesNumber];
                    string newSopInstanceUid = DicomUid.GenerateUid().UID;

                    // Change a new Dicom file
                    DicomFile dicomFile = new DicomFile(thisSop.LocationUri.LocalDiskPath);
                    dicomFile.Load(DicomReadOptions.Default);
                    dicomFile.DataSet[DicomTags.StudyInstanceUid].SetStringValue(newStudyInstanceUid);
                    dicomFile.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(newSeriesInstanceUid);
                    dicomFile.DataSet[DicomTags.SopInstanceUid].SetStringValue(newSopInstanceUid);
                    dicomFile.DataSet[DicomTags.PatientsName].SetStringValue(String.Format("{0}^{1}", profile.Name.FamilyName, profile.Name.GivenName));
                    dicomFile.DataSet[DicomTags.PatientId].SetStringValue(String.Format("{0}{1}", profile.Mrn.AssigningAuthority, profile.Mrn.Id));
                    dicomFile.DataSet[DicomTags.PatientsSex].SetStringValue(profile.Sex.Code);
                    dicomFile.DataSet[DicomTags.AccessionNumber].SetStringValue(order.AccessionNumber);
                    dicomFile.DataSet[DicomTags.PatientsBirthDate].SetStringValue(RandomUtils.FormatDateTime(profile.DateOfBirth.Value, "YYYYMMDD"));
                    dicomFile.Filename = String.Format("{0}\\{1}.dcm", Directory.GetCurrentDirectory(), newSopInstanceUid);
                    dicomFile.Save(DicomWriteOptions.Default);

                    filePaths.Add(dicomFile.Filename);
                }
            }

            return filePaths;
        }

        private static void ImportFiles(IEnumerable<string> filePaths)
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
