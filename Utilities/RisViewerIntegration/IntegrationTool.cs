#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;
using ClearCanvas.Ris.Client;
using ClearCanvas.Utilities.DicomEditor;

namespace ClearCanvas.Utilities.RisViewerIntegration
{
	[ButtonAction("apply", "dicomstudybrowser-toolbar/Integration", "Apply")]
	[MenuAction("apply", "dicomstudybrowser-contextmenu/Integration", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[Tooltip("apply", "Integration")]
	[IconSet("apply", IconScheme.Colour, "Icons.IntegrationToolSmall.png", "Icons.IntegrationToolSmall.png", "Icons.IntegrationToolSmall.png")]

	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.DemoAdmin)]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class IntegrationTool : StudyBrowserTool, IAnonymizationCallback
	{
		private volatile string _currentPatientSex;
		private volatile int _anonymizingPriorNumber;
		private volatile IBackgroundTaskContext _context;

		#region IAnonymizationCallback Members

		void IAnonymizationCallback.BeforeAnonymize(DicomAttributeCollection dataSet)
		{
			dataSet[DicomTags.PatientsSex].SetStringValue(_currentPatientSex);
		}

		void IAnonymizationCallback.BeforeSave(DicomAttributeCollection dataSet)
		{
		}

		void IAnonymizationCallback.ReportProgress(int percent, string message)
		{
			if (_anonymizingPriorNumber == 0)
				_context.ReportProgress(new BackgroundTaskProgress(percent, "Duplicating study with anonymized patient data..."));
			else
				_context.ReportProgress(new BackgroundTaskProgress(percent, 
					String.Format("Duplicating prior#{0} with anonymized patient data...", _anonymizingPriorNumber)));
		}

		#endregion

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			Enabled = this.Context.SelectedStudy != null && this.Context.SelectedServerGroup.IsLocalDatastore;
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			Enabled = this.Context.SelectedStudy != null && this.Context.SelectedServerGroup.IsLocalDatastore;
		}

		public void Apply()
		{
			BackgroundTask task = null;

			try
			{
				task = new BackgroundTask(IntegrationMethod, false);
				ProgressDialog.Show(task, this.Context.DesktopWindow, true);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
			finally
			{
				_currentPatientSex = null;
				_context = null;
				
				if (task != null)
					task.Dispose();
			}
		}

		private void IntegrationMethod(IBackgroundTaskContext context)
		{
			_context = context;

			context.ReportProgress(new BackgroundTaskProgress(0, "Find all studies in viewer database..."));

			int step = 0;
			int studyIndex = 0;
			int totalStep = 6 * this.Context.SelectedStudies.Count;

			List<Exception> failedException = new List<Exception>();
			foreach (StudyItem study in this.Context.SelectedStudies)
			{
				try
				{
					studyIndex++;

					string commonMessage = String.Format("Study #{0}: {1} {2}\r\n", studyIndex, study.PatientsName.FirstName, study.PatientsName.LastName);

					context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Find diagnostic service name with study description..."));
					string diagnosticServiceName = GetDiagnosticServiceName(study.StudyDescription);

					context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate an anonymized patient in Ris database..."));
					PatientProfileSummary profile = RandomUtils.CreatePatient();

					context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate a visit..."));
					VisitSummary visit = RandomUtils.CreateVisit(profile.PatientRef, profile.Mrn.AssigningAuthority, 0);

					context.ReportProgress(new BackgroundTaskProgress(CalculatePercentage(step++, totalStep), commonMessage + "Generate an order..."));
					string newOrderAccessionNumber = GenerateRandomOrder(profile, diagnosticServiceName, visit);

					_currentPatientSex = profile.Sex.Code;

					AnonymizeStudyAndPriors(study, profile, newOrderAccessionNumber);
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

		private void AnonymizeStudyAndPriors(StudyItem study, PatientProfileSummary profile, string newOrderAccessionNumber)
		{
			string patientId = String.Format("{0}{1}", profile.Mrn.AssigningAuthority, profile.Mrn.Id);
			string patientsName = String.Format("{0}^{1}", profile.Name.GivenName, profile.Name.FamilyName);

			_anonymizingPriorNumber = 0;

			AnonymizationHelper.AnonymizeLocalStudy(study.StudyInstanceUID,
											patientId,
											patientsName,
											profile.DateOfBirth.Value,
											newOrderAccessionNumber,
											study.StudyDescription,
											Platform.Time, true, true, this);

			IStudyFinder studyFinder =
			(IStudyFinder)CollectionUtils.SelectFirst(new StudyFinderExtensionPoint().CreateExtensions(),
												delegate(object test)
												{
													return ((IStudyFinder)test).Name == "DICOM_LOCAL";
												});
			QueryParameters queryParams = new QueryParameters();
			queryParams.Add("PatientsName", "");
			queryParams.Add("PatientId", study.PatientId);
			queryParams.Add("AccessionNumber", "");
			queryParams.Add("StudyDescription", "");
			queryParams.Add("ModalitiesInStudy", "");
			queryParams.Add("StudyDate", "");
			queryParams.Add("StudyInstanceUid", "");

			StudyItemList priors = studyFinder.Query(queryParams, null);
			if (priors == null)
				return;

			Random random = new Random();
			foreach (StudyItem prior in priors)
			{
				if (prior.StudyInstanceUID != study.StudyInstanceUID)
				{
					++_anonymizingPriorNumber;

					DateTime? studyDate = DateParser.Parse(study.StudyDate);
					DateTime? priorStudyDate = DateParser.Parse(prior.StudyDate);
					
					TimeSpan trueDiff = TimeSpan.FromDays(120);
					if (studyDate != null && priorStudyDate != null)
						trueDiff = studyDate.Value - priorStudyDate.Value;

					// change it to be +- 3 months from the 'true' difference.
					priorStudyDate = (Platform.Time - trueDiff).AddDays(-random.Next(0, 90));

					AnonymizationHelper.AnonymizeLocalStudy(prior.StudyInstanceUID,
											patientId,
											patientsName,
											profile.DateOfBirth.Value,
											"",
											prior.StudyDescription,
											priorStudyDate, true, true, this);
				}
			}
		}

		#region Other helper functions

		private static int CalculatePercentage(int step, int totalStep)
		{
			decimal result = (decimal)step / totalStep * 100;
			return (int)Math.Floor(result);
		}

		#endregion

		#region Ris Related Functions

		private static string GetDiagnosticServiceName(string studyDescription)
		{
			string diagnosticServiceName = "";

			// Using the study description, look for a corresponding diagnostic service name in a CSV mapping file
			using (TextReader reader = new StreamReader("Dictionary.DiagnosticServices.csv"))
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
				throw new Exception(String.Format("Failed to find a Diagnostic Service mapping for study description: {0}", studyDescription));

			List<DiagnosticServiceSummary> diagnosticServiceChoices = null;
			Platform.GetService<IDiagnosticServiceAdminService>(
				delegate(IDiagnosticServiceAdminService service)
					{
						// get the diagnostic service by name, or if name is null, just load the first 100
						// so that we can choose a random one
						ListDiagnosticServicesRequest request = new ListDiagnosticServicesRequest(diagnosticServiceName, null);
						request.Page.FirstRow = 0;
						request.Page.MaxRows = 100;
						diagnosticServiceChoices = service.ListDiagnosticServices(request).DiagnosticServices;
					});

			if (diagnosticServiceChoices == null || diagnosticServiceChoices.Count == 0)
				throw new Exception(String.Format("Cannot find diagnostic service with name {0}", diagnosticServiceName));

			return diagnosticServiceName;
		}

		private static string GenerateRandomOrder(PatientProfileSummary profile, string diagnosticServiceName, VisitSummary visit)
		{
			OrderSummary orderSummary = RandomUtils.RandomOrder(visit, profile.Mrn.AssigningAuthority, diagnosticServiceName, 0);
			string accessionNumber = orderSummary.AccessionNumber;
			EntityRef orderRef = orderSummary.OrderRef;

			Platform.GetService<IModalityWorkflowService>(
				delegate(IModalityWorkflowService service)
				{
					QueryWorklistRequest queryRequest = new QueryWorklistRequest(WorklistClassNames.TechnologistScheduledWorklist, true, true);
					QueryWorklistResponse<ModalityWorklistItem> queryResponse = service.QueryWorklist(queryRequest);
					List<EntityRef> procedureStepsRef = new List<EntityRef>();
					foreach (ModalityWorklistItem item in queryResponse.WorklistItems)
					{
						if (item.OrderRef == orderSummary.OrderRef)
							procedureStepsRef.Add(item.ProcedureStepRef);
					}

					StartModalityProcedureStepsRequest request = new StartModalityProcedureStepsRequest(procedureStepsRef);
					StartModalityProcedureStepsResponse response = service.StartModalityProcedureSteps(request);

					CompleteModalityPerformedProcedureStepRequest completeRequest =
						new CompleteModalityPerformedProcedureStepRequest(
							response.StartedMpps.ModalityPerformendProcedureStepRef,
							response.StartedMpps.ExtendedProperties);

					CompleteModalityPerformedProcedureStepResponse rsp = service.CompleteModalityPerformedProcedureStep(completeRequest);
					orderRef = rsp.ProcedurePlan.OrderRef;
				});

			Platform.GetService<ITechnologistDocumentationService>(
				delegate(ITechnologistDocumentationService service)
					{
						service.CompleteOrderDocumentation(new CompleteOrderDocumentationRequest(orderRef));
					});

			return accessionNumber;
		}

		#endregion
	}
}