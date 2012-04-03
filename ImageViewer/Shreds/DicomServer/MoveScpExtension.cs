#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    //TODO (Marmot):Restore/rewrite.
    /*
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class MoveScpExtension : ScpExtension
	{
		#region SendOperationInfo class

		private class SendOperationInfo
		{
			public SendOperationInfo(SendOperationReference reference, ushort messageId, byte presentationId, ClearCanvas.Dicom.Network.DicomServer server)
			{
				this.PresentationId = presentationId;
				this.Server = server;
				this.Reference = reference;
				this.MessageId = messageId;
			}

			public readonly SendOperationReference Reference;

			public readonly ushort MessageId;
			public readonly byte PresentationId;
			public readonly ClearCanvas.Dicom.Network.DicomServer Server;
		}

		#endregion

		#region Private Fields

		private readonly object _syncLock = new object();
		//There will only be one of these at a time, but I'm paranoid.
		private readonly List<SendOperationInfo> _sendOperations;

		#endregion

		public MoveScpExtension()
			: base(GetSupportedSops())
		{
			_sendOperations = new List<SendOperationInfo>();
		}

		#region Private Methods

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			SupportedSop sop = new SupportedSop();
			sop.SopClass = SopClass.StudyRootQueryRetrieveInformationModelMove;
			sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
			sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
			yield return sop;
		}

		private SendOperationInfo GetSendOperationInfo(SendOperationReference reference)
		{
			lock (_syncLock)
			{
				return CollectionUtils.SelectFirst(_sendOperations,
					delegate(SendOperationInfo info) { return info.Reference == reference; });
			}
		}

		private SendOperationInfo GetSendOperationInfo(int dicomMessageId)
		{
			lock (_syncLock)
			{
				return CollectionUtils.SelectFirst(_sendOperations,
					delegate(SendOperationInfo info) { return info.MessageId == dicomMessageId; });
			}
		}

		private void RemoveSendOperationInfo(SendOperationInfo info)
		{
			lock(_syncLock)
			{
				_sendOperations.Remove(info);
			}
		}

		private void UpdateProgress(ISendOperation operation)
		{
			SendOperationInfo sendOperationInfo = GetSendOperationInfo(operation.Reference);
			if (sendOperationInfo == null)
			{
				// there are some conditions that both throw an exception as well as fails remaining items, so we'll just ignore the second attempt to send a final response
				// namely, the catch handler block for the call to Connect() in StorageScu.Send()
				if (!operation.Failed)
					Platform.Log(LogLevel.Debug, "Received progress update for unknown send operation {0}", operation.Reference.Identifier);
				return;
			}

			DicomMessage msg = new DicomMessage();
			DicomStatus status;

			if (operation.Failed)
			{
				RemoveSendOperationInfo(sendOperationInfo);

				status = DicomStatuses.QueryRetrieveUnableToProcess;
			}
			else if (operation.RemainingSubOperations == 0)
			{
				RemoveSendOperationInfo(sendOperationInfo);

				foreach (StorageInstance sop in operation.StorageInstances)
				{
					if ((sop.SendStatus.Status != DicomState.Success) && (sop.SendStatus.Status != DicomState.Warning))
						msg.DataSet[DicomTags.FailedSopInstanceUidList].AppendString(sop.SopInstanceUid);
				}

				if (operation.Canceled)
					status = DicomStatuses.Cancel;
				else if (operation.FailureSubOperations > 0)
					status = DicomStatuses.QueryRetrieveSubOpsOneOrMoreFailures;
				else
					status = DicomStatuses.Success;
			}
			else
			{
				status = DicomStatuses.Pending;
				if ((operation.RemainingSubOperations % 5) != 0)
					return;

				// Only send a RSP every 5 to reduce network load
			}

			sendOperationInfo.Server.SendCMoveResponse(sendOperationInfo.PresentationId, sendOperationInfo.MessageId,
			                         msg, status,
									 (ushort)operation.SuccessSubOperations,
									 (ushort)operation.RemainingSubOperations,
									 (ushort)operation.FailureSubOperations,
									 (ushort)operation.WarningSubOperations);
		}

		private void OnReceiveMoveStudiesRequest(ClearCanvas.Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, ApplicationEntity remoteAEInfo)
		{
			IEnumerable<string> studyUids = (string[])message.DataSet[DicomTags.StudyInstanceUid].Values;
			SendStudiesRequest request = new SendStudiesRequest();
			request.StudyInstanceUids = studyUids;
			request.DestinationAEInformation = remoteAEInfo;

			AuditedInstances instances = new AuditedInstances();
			EventResult result = EventResult.Success;

			lock (_syncLock)
			{
				try
				{
					foreach (IStudy study in DataStoreQueryHelper.GetStudies(studyUids))
						instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

					SendOperationReference reference = DicomSendManager.Instance.SendStudies(request, UpdateProgress);
					_sendOperations.Add(new SendOperationInfo(reference, message.MessageId, presentationID, server));
				}
				catch
				{
					result = EventResult.MajorFailure;
					throw;
				}
				finally
				{
					AuditHelper.LogBeginSendInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName, instances, EventSource.CurrentProcess, result);
				}
			}
		}

        private void OnReceiveMoveSeriesRequest(ClearCanvas.Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, ApplicationEntity remoteAEInfo)
		{
			string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
			string[] seriesUids = (string[])message.DataSet[DicomTags.SeriesInstanceUid].Values;
			SendSeriesRequest request = new SendSeriesRequest();
			request.DestinationAEInformation = remoteAEInfo;
			request.StudyInstanceUid = studyInstanceUid;
			request.SeriesInstanceUids = seriesUids;

			AuditedInstances instances = new AuditedInstances();
			EventResult result = EventResult.Success;

			lock (_syncLock)
			{
				try
				{
					IStudy study = DataStoreQueryHelper.GetStudy(studyInstanceUid);
					instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

					SendOperationReference reference = DicomSendManager.Instance.SendSeries(request, UpdateProgress);
					_sendOperations.Add(new SendOperationInfo(reference, message.MessageId, presentationID, server));
				}
				catch
				{
					result = EventResult.MajorFailure;
					throw;
				}
				finally
				{
					AuditHelper.LogBeginSendInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName, instances, EventSource.CurrentProcess, result);
				}
			}
		}

        private void OnReceiveMoveImageRequest(ClearCanvas.Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, ApplicationEntity remoteAEInfo)
		{
			string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
			string seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
			string[] sopInstanceUids = (string[])message.DataSet[DicomTags.SopInstanceUid].Values;
			SendSopInstancesRequest request = new SendSopInstancesRequest();
			request.DestinationAEInformation = remoteAEInfo;
			request.StudyInstanceUid = studyInstanceUid;
			request.SeriesInstanceUid = seriesInstanceUid;
			request.SopInstanceUids = sopInstanceUids;

			EventResult result = EventResult.Success;
			AuditedInstances instances = new AuditedInstances();

			lock (_syncLock)
			{
				try
				{
					IStudy study = DataStoreQueryHelper.GetStudy(studyInstanceUid);
					instances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

					SendOperationReference reference = DicomSendManager.Instance.SendSopInstances(request, UpdateProgress);
					_sendOperations.Add(new SendOperationInfo(reference, message.MessageId, presentationID, server));
				}
				catch
				{
					result = EventResult.MajorFailure;
					throw;
				}
				finally
				{
					AuditHelper.LogBeginSendInstances(remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName, instances, EventSource.CurrentProcess, result);
				}
			}
		}

		private void OnReceiveCancelRequest(DicomMessage message)
		{
			lock (_syncLock)
			{
				SendOperationInfo sendOperationInfo = GetSendOperationInfo(message.MessageIdBeingRespondedTo);
				if (sendOperationInfo != null)
				{
					RemoveSendOperationInfo(sendOperationInfo);
					DicomSendManager.Instance.Cancel(sendOperationInfo.Reference);
				}
				else
				{
					Platform.Log(LogLevel.Warn, "Received C-CANCEL-MOVE-RQ for unknown message id {0}",
						message.MessageIdBeingRespondedTo);
				}
			}
		}

		#endregion

		#region Overrides

		public override bool OnReceiveRequest(ClearCanvas.Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			//// Check for a Cancel message, and cancel the SCU.
			if (message.CommandField == DicomCommandField.CCancelRequest)
			{
				OnReceiveCancelRequest(message);
				return true;
			}

			var remoteAE = RemoteServerDirectory.Lookup(message.MoveDestination);
            if (remoteAE == null)
			{
				server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
					DicomStatuses.QueryRetrieveMoveDestinationUnknown);
				return true;
			}

			String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, "");

			try
			{
				if (level.Equals("STUDY"))
				{
                    OnReceiveMoveStudiesRequest(server, presentationID, message, remoteAE.ToDataContract());
				}
				else if (level.Equals("SERIES"))
				{
                    OnReceiveMoveSeriesRequest(server, presentationID, message, remoteAE.ToDataContract());
				}
				else if (level.Equals("IMAGE"))
				{
                    OnReceiveMoveImageRequest(server, presentationID, message, remoteAE.ToDataContract());
				}
				else
				{
					Platform.Log(LogLevel.Error, "Unexpected Study Root Move Query/Retrieve level: {0}", level);

					server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
					                         DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
					return true;
				}
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing C-MOVE-RQ");
				try
				{
					server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
											 DicomStatuses.QueryRetrieveUnableToProcess);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex,
								 "Unable to send final C-MOVE-RSP message on association from {0} to {1}",
								 association.CallingAE, association.CalledAE);
				}
			}

			return true;
		}

		#endregion
	}
     */
}
