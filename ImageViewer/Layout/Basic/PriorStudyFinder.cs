using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[ExceptionPolicyFor(typeof(PriorStudyLoaderException))]
	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public class PriorStudyLoaderExceptionPolicy : IExceptionPolicy
	{
		public PriorStudyLoaderExceptionPolicy()
		{
		}

		#region IExceptionPolicy Members

		public void Handle(System.Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			if (e is PriorStudyLoaderException)
				Handle(e as PriorStudyLoaderException, exceptionHandlingContext);
		}

		#endregion

		private static void Handle(PriorStudyLoaderException exception, IExceptionHandlingContext context)
		{
			if (exception.FindFailed || (exception.CompleteFailures == exception.TotalQueryResults))
			{
				context.ShowMessageBox(SR.MessageFailedToLoadAnyPriors);
			}
			else if (exception.CompleteFailures > 0 && exception.PartialFailures > 0)
			{
				string message = String.Format(SR.FormatXCompleteYPartialPriorLoadFailures, 
					exception.CompleteFailures, exception.PartialFailures);

				context.ShowMessageBox(message);
			}
			else if (exception.CompleteFailures > 0)
			{
				string message;
				if (exception.CompleteFailures == 1)
					message = SR.Message1CompletePriorLoadFailures;
				else
					message = String.Format(SR.FormatXCompletePriorLoadFailures, exception.CompleteFailures);

				context.ShowMessageBox(message);
			}
			else if (exception.PartialFailures > 0)
			{
				string message;
				if (exception.PartialFailures == 1)
					message = SR.Message1PartialPriorLoadFailures;
				else
					message = String.Format(SR.FormatXPartialPriorLoadFailures, exception.PartialFailures);

				context.ShowMessageBox(message);
			}
			else if (exception.NoStudyLoaderFailures > 0)
			{
				//ignore, since there's not a lot we can do about it.
			}
			else
			{
				context.ShowMessageBox(SR.MessageUnexpectedPriorLoadFailure);
			}
		}
	}

	[ExtensionOf(typeof(PriorStudyFinderExtensionPoint))]
	public class PriorStudyFinder : ClearCanvas.ImageViewer.PriorStudyFinder
	{
		private volatile bool _cancel;

		public PriorStudyFinder()
		{
		}

		public override StudyItemList FindPriorStudies()
		{
			_cancel = false;
			StudyItemList results = new StudyItemList();

			DefaultPatientReconciliationStrategy reconciliationStrategy = new DefaultPatientReconciliationStrategy();
			List<string> patientIds = new List<string>();
			foreach (Patient patient in Viewer.StudyTree.Patients)
			{
				if (_cancel)
					break;

				PatientInformation info = new PatientInformation(patient);
				PatientInformation reconciled = reconciliationStrategy.ReconcilePatient(info);
				if (!patientIds.Contains(reconciled.PatientId))
					patientIds.Add(reconciled.PatientId);
			}

			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
			{
				foreach (string patientId in patientIds)
				{
					StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
					identifier.PatientId = patientId;
					if (DefaultPatientReconciliationSettings.Default.PatientIdSearchAppendWildcard)
						identifier.PatientId = identifier.PatientId  + "*";

					IList<StudyRootStudyIdentifier> studies = bridge.StudyQuery(identifier);
					foreach (StudyRootStudyIdentifier study in studies)
					{
						StudyItem studyItem = ConvertToStudyItem(study);
						if (studyItem != Null)
							results.Add(studyItem);
					}
				}
			}

			return results;
		}

		public override void Cancel()
		{
			_cancel = true;
		}

		private StudyItem ConvertToStudyItem(StudyRootStudyIdentifier study)
		{
			StudyItem item = new StudyItem();
			IServerTreeNode node = FindServer(study.RetrieveAeTitle);
			if (node.IsLocalDataStore)
			{
				item.StudyLoaderName = "DICOM_LOCAL";
			}
			else if (node.IsServer)
			{
				Server server = (Server) node;
				if (server.IsStreaming)
					item.StudyLoaderName = "CC_STREAMING";
				else
					item.StudyLoaderName = "DICOM_REMOTE";

				item.Server = new ApplicationEntity(server.Host, server.AETitle, server.Port, server.HeaderServicePort, server.WadoServicePort);
			}
			else // (node == null)
			{
				Platform.Log(LogLevel.Warn,
					String.Format("Unable to find server information '{0}' in order to load study '{1}'",
					study.RetrieveAeTitle, study.StudyInstanceUid));

				return null;
			}

			item.AccessionNumber = study.AccessionNumber;
			item.ModalitiesInStudy = DicomStringHelper.GetDicomStringArray(study.ModalitiesInStudy ?? new string[0]);
			item.NumberOfStudyRelatedInstances = (uint)(study.NumberOfStudyRelatedInstances ?? 0);
			item.PatientId = study.PatientId;
			item.PatientsBirthDate = study.PatientsBirthDate;
			item.PatientsName = new PersonName(study.PatientsName);
			item.SpecificCharacterSet = study.SpecificCharacterSet;
			item.StudyDate = study.StudyDate;
			item.StudyDescription = study.StudyDescription;
			item.StudyInstanceUID = study.StudyInstanceUid;

			return item;
		}

		private static IServerTreeNode FindServer(string retrieveAETitle)
		{
			ServerTree serverTree = new ServerTree();
			if (retrieveAETitle == serverTree.RootNode.LocalDataStoreNode.GetClientAETitle())
				return serverTree.RootNode.LocalDataStoreNode;

			List<Server> remoteServers = Configuration.DefaultServers.SelectFrom(serverTree);
			foreach (Server server in remoteServers)
			{
				if (server.AETitle == retrieveAETitle)
					return server;
			}

			return null;
		}
	}
}
