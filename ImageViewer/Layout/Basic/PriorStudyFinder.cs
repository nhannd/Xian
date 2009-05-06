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
				PatientInformation reconciled = reconciliationStrategy.ReconcileSearchCriteria(info);
				if (!patientIds.Contains(reconciled.PatientId))
					patientIds.Add(reconciled.PatientId);
			}

			using (StudyRootQueryBridge bridge = new StudyRootQueryBridge(Platform.GetService<IStudyRootQuery>()))
			{
				foreach (string patientId in patientIds)
				{
					StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
					identifier.PatientId = patientId;

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
			string studyLoaderName;
			ApplicationEntity applicationEntity = null;

			IServerTreeNode node = FindServer(study.RetrieveAeTitle);
			if (node.IsLocalDataStore)
			{
				studyLoaderName = "DICOM_LOCAL";
			}
			else if (node.IsServer)
			{
				Server server = (Server)node;
				if (server.IsStreaming)
					studyLoaderName = "CC_STREAMING";
				else
					studyLoaderName = "DICOM_REMOTE";

				applicationEntity = new ApplicationEntity(server.Host, server.AETitle, server.Name, server.Port,
											server.IsStreaming, server.HeaderServicePort, server.WadoServicePort);
			}
			else // (node == null)
			{
				Platform.Log(LogLevel.Warn,
					String.Format("Unable to find server information '{0}' in order to load study '{1}'",
					study.RetrieveAeTitle, study.StudyInstanceUid));

				return null;
			}

			StudyItem item = new StudyItem(study.StudyInstanceUid, applicationEntity, studyLoaderName);
			item.AccessionNumber = study.AccessionNumber;
			item.ModalitiesInStudy = DicomStringHelper.GetDicomStringArray(study.ModalitiesInStudy ?? new string[0]);
			item.NumberOfStudyRelatedInstances = (uint)(study.NumberOfStudyRelatedInstances ?? 0);
			item.PatientId = study.PatientId;
			item.PatientsBirthDate = study.PatientsBirthDate;
			item.PatientsName = new PersonName(study.PatientsName);
			item.SpecificCharacterSet = study.SpecificCharacterSet;
			item.StudyDate = study.StudyDate;
			item.StudyDescription = study.StudyDescription;

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
