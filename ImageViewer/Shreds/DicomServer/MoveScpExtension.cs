using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Services;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class MoveScpExtension : ScpExtension, IDicomScp<IDicomServerContext>
	{
		#region SendOperationInfo class

		private class SendOperationInfo
		{
			public SendOperationInfo(Guid identifier, ushort messageId, byte presentationId, Dicom.Network.DicomServer server)
			{
				this.PresentationId = presentationId;
				this.Server = server;
				this.Identifier = identifier;
				this.MessageId = messageId;
			}

			public readonly Guid Identifier;

			public readonly ushort MessageId;
			public readonly byte PresentationId;
			public readonly Dicom.Network.DicomServer Server;
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

		private SendOperationInfo GetSendOperationInfo(Guid operationIdentifier)
		{
			lock (_syncLock)
			{
				return CollectionUtils.SelectFirst(_sendOperations,
					delegate(SendOperationInfo info) { return info.Identifier == operationIdentifier; });
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
			SendOperationInfo sendOperationInfo = GetSendOperationInfo(operation.Identifier);
			if (sendOperationInfo == null)
			{
				Platform.Log(LogLevel.Warn, "Received progress update for unknown send operation {0}", operation.Identifier);
				return;
			}

			DicomMessage msg = new DicomMessage();
			DicomStatus status;

			if (operation.RemainingSubOperations == 0)
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

		private void OnReceiveMoveStudiesRequest(Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, AEInformation remoteAEInfo)
		{
			IEnumerable<string> studyUids = (string[])message.DataSet[DicomTags.StudyInstanceUid].Values;
			SendStudiesRequest request = new SendStudiesRequest(remoteAEInfo, studyUids, UpdateProgress);
			lock (_syncLock)
			{
				Guid sendOperationIdentifier = DicomSendManager.Instance.SendStudies(request);
				_sendOperations.Add(new SendOperationInfo(sendOperationIdentifier, message.MessageId, presentationID, server));
			}
		}

		private void OnReceiveMoveSeriesRequest(Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, AEInformation remoteAEInfo)
		{
			string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
			string[] seriesUids = (string[])message.DataSet[DicomTags.SeriesInstanceUid].Values;
			SendSeriesRequest request = new SendSeriesRequest(remoteAEInfo, studyInstanceUid, seriesUids, UpdateProgress);

			lock (_syncLock)
			{
				Guid sendOperationIdentifier = DicomSendManager.Instance.SendSeries(request);
				_sendOperations.Add(new SendOperationInfo(sendOperationIdentifier, message.MessageId, presentationID, server));
			}
		}

		private void OnReceiveMoveImageRequest(Dicom.Network.DicomServer server, byte presentationID, DicomMessage message, AEInformation remoteAEInfo)
		{
			string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
			string seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
			string[] sopInstanceUids = (string[])message.DataSet[DicomTags.SopInstanceUid].Values;
			SendSopInstancesRequest request = new SendSopInstancesRequest(remoteAEInfo, studyInstanceUid, seriesInstanceUid, sopInstanceUids, UpdateProgress);

			lock (_syncLock)
			{
				Guid sendOperationIdentifier = DicomSendManager.Instance.SendSopInstances(request);
				_sendOperations.Add(new SendOperationInfo(sendOperationIdentifier, message.MessageId, presentationID, server));
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
					DicomSendManager.Instance.Cancel(sendOperationInfo.Identifier);
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

		public override bool OnReceiveRequest(Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			//// Check for a Cancel message, and cancel the SCU.
			if (message.CommandField == DicomCommandField.CCancelRequest)
			{
				OnReceiveCancelRequest(message);
				return true;
			}

			AEInformation remoteAEInfo = RemoteServerDirectory.Lookup(message.MoveDestination);
			if (remoteAEInfo == null)
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
					OnReceiveMoveStudiesRequest(server, presentationID, message, remoteAEInfo);
				}
				else if (level.Equals("SERIES"))
				{
					OnReceiveMoveSeriesRequest(server, presentationID, message, remoteAEInfo);
				}
				else if (level.Equals("IMAGE"))
				{
					OnReceiveMoveImageRequest(server, presentationID, message, remoteAEInfo);
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
}
