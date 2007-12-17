#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.OffisNetwork;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Threading;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal sealed partial class DicomServerManager : IDicomServerService
	{
		private static DicomServerManager _instance;

		// Used by CFindScp
		private Dictionary<uint, DicomQuerySession> _querySessionDictionary;
		private object _querySessionLock = new object();

		// Used by CMoveScp to keep track of the sub-CStore progerss
		private Dictionary<uint, DicomMoveSession> _moveSessionDictionary;
		private object _moveSessionLock = new object();

		private List<BackgroundTaskContainer> _sendRetrieveTasks;
		private object _sendRetrieveTaskLock = new object();

        private volatile ClearCanvas.Dicom.OffisNetwork.DicomServer _dicomServer;

		private object _restartServerLock = new object();
		private bool _restartingServer;

		private object _serverConfigurationLock = new object();

		public DicomServerManager()
		{
			_querySessionDictionary = new Dictionary<uint, DicomQuerySession>();
			_moveSessionDictionary = new Dictionary<uint, DicomMoveSession>();
			_sendRetrieveTasks = new List<BackgroundTaskContainer>();
			_restartingServer = false;
		}

		public static DicomServerManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new DicomServerManager();

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		public void Start()
		{
			DicomEventManager.Instance.FindScpEvent += OnFindScpEvent;
			DicomEventManager.Instance.FindScpProgressEvent += OnFindScpProgressEvent;
			DicomEventManager.Instance.StoreScpBeginEvent += OnStoreScpBeginEvent;
			DicomEventManager.Instance.StoreScpProgressEvent += OnStoreScpProgressEvent;
			DicomEventManager.Instance.StoreScpEndEvent += OnStoreScpEndEvent;
			DicomEventManager.Instance.MoveScpBeginEvent += OnMoveScpBeginEvent;
			DicomEventManager.Instance.MoveScpProgressEvent += OnMoveScpProgressEvent;
			DicomEventManager.Instance.StoreScuProgressEvent += OnStoreScuProgressEvent;

			StartServer();
		}

		public void Stop()
		{
			lock (_restartServerLock)
			{
				_restartingServer = false;
				StopServer();
			}

			DicomEventManager.Instance.FindScpEvent -= OnFindScpEvent;
			DicomEventManager.Instance.FindScpProgressEvent -= OnFindScpProgressEvent;
			DicomEventManager.Instance.StoreScpBeginEvent -= OnStoreScpBeginEvent;
			DicomEventManager.Instance.StoreScpProgressEvent -= OnStoreScpProgressEvent;
			DicomEventManager.Instance.StoreScpEndEvent -= OnStoreScpEndEvent;
			DicomEventManager.Instance.MoveScpBeginEvent -= OnMoveScpBeginEvent;
			DicomEventManager.Instance.MoveScpProgressEvent -= OnMoveScpProgressEvent;
			DicomEventManager.Instance.StoreScuProgressEvent -= OnStoreScuProgressEvent;

			_querySessionDictionary.Clear();
			_moveSessionDictionary.Clear();
			_sendRetrieveTasks.Clear();
		}

		private void StopServer()
		{
			if (_dicomServer != null)
			{
				_dicomServer.Stop();
				_dicomServer = null;
			}
		}

		private void StartServer()
		{
			lock (_serverConfigurationLock)
			{
				// Create storage directory
				if (!Directory.Exists(DicomServerSettings.Instance.InterimStorageDirectory))
					Directory.CreateDirectory(DicomServerSettings.Instance.InterimStorageDirectory);

				ApplicationEntity myApplicationEntity = new ApplicationEntity(
					new HostName(DicomServerSettings.Instance.HostName),
					new AETitle(DicomServerSettings.Instance.AETitle),
					new ListeningPort(DicomServerSettings.Instance.Port));

                _dicomServer = new ClearCanvas.Dicom.OffisNetwork.DicomServer(myApplicationEntity, DicomServerSettings.Instance.InterimStorageDirectory);
			}

			_dicomServer.Start();
		}

		#region Properties

		#endregion

		#region DicomServer FindScp Event Handlers

		private void OnFindScpEvent(object sender, DicomEventArgs e)
		{
			InteropFindScpCallbackInfo info = new InteropFindScpCallbackInfo(e.CallbackInfoPointer, false);

			QueryDB(info.QueryRetrieveOperationIdentifier, info.RequestIdentifiers);
			info.Response.DimseStatus = (ushort)OffisDcm.STATUS_Pending;
		}

		private void OnFindScpProgressEvent(object sender, DicomEventArgs e)
		{
			InteropFindScpCallbackInfo info = new InteropFindScpCallbackInfo(e.CallbackInfoPointer, false);

			info.Response.DimseStatus = (ushort)GetNextQueryResult(info.QueryRetrieveOperationIdentifier, info.ResponseIdentifiers);
		}

		#endregion

		#region DicomServer MoveScp Event Handlers

		private void OnMoveScpBeginEvent(object sender, DicomEventArgs e)
		{
			InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);

			DicomMoveSession moveSession;

			lock (_moveSessionLock)
			{
				moveSession = new DicomMoveSession(null, null);
				_moveSessionDictionary[info.QueryRetrieveOperationIdentifier] = moveSession;
			}

			DicomQuerySession querySession = QueryDB(info.QueryRetrieveOperationIdentifier, info.RequestIdentifiers);
			if (querySession.DimseStatus != OffisDcm.STATUS_Pending)
			{
				moveSession.Status = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
				return;
			}

			ApplicationEntity destinationAE = null;

			try
			{
				ClearCanvas.ImageViewer.Services.ServerTree.ServerTree serverTree = new ClearCanvas.ImageViewer.Services.ServerTree.ServerTree();
				List<ClearCanvas.ImageViewer.Services.ServerTree.IServerTreeNode> servers = serverTree.FindChildServers(serverTree.RootNode.ServerGroupNode as ClearCanvas.ImageViewer.Services.ServerTree.ServerGroup);
				foreach (ClearCanvas.ImageViewer.Services.ServerTree.Server server in servers)
				{
					if (server.AETitle == info.Request.MoveDestination)
					{
						destinationAE = new ApplicationEntity(new HostName(server.Host), new AETitle(server.AETitle), new ListeningPort(server.Port));
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Error, ex);
				destinationAE = null;
			}

			if (destinationAE == null)
			{
				moveSession.Status = (ushort)OffisDcm.STATUS_MOVE_Failed_MoveDestinationUnknown;
				return;
			}

			ApplicationEntity myApplicationEntity = new ApplicationEntity(new HostName(DicomServerSettings.Instance.HostName), new AETitle(DicomServerSettings.Instance.AETitle), new ListeningPort(DicomServerSettings.Instance.Port));
			SendParcel sendParcel = new SendParcel(myApplicationEntity, destinationAE, "");

			// Move all return results
			while (info.Response.DimseStatus == (ushort)OffisDcm.STATUS_Pending)
			{
				info.Response.DimseStatus = (ushort)GetNextQueryResult(info.QueryRetrieveOperationIdentifier, info.ResponseIdentifiers);
				if (info.Response.DimseStatus != (ushort)OffisDcm.STATUS_Pending)
					break;

				string studyInstanceUID;
				OffisDicomHelper.TryFindAndGetOFString(info.RequestIdentifiers, Dcm.StudyInstanceUID, out studyInstanceUID);
				if (String.IsNullOrEmpty(studyInstanceUID))
				{
					lock (_querySessionLock)
					{
						_querySessionDictionary.Remove(info.QueryRetrieveOperationIdentifier);
					}

					moveSession.Status = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
					return;
				}

				sendParcel.Include(new Uid(studyInstanceUID));
			}

			if (sendParcel.GetToSendObjectCount() == 0)
			{
				moveSession.Status = (ushort)OffisDcm.STATUS_Success;
				return;
			}

			BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext context)
			{
				try
				{
					sendParcel.Send();
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex);
				}

			}, false);

			lock (_moveSessionLock)
			{
				moveSession = new DicomMoveSession(sendParcel, task);
				_moveSessionDictionary[info.QueryRetrieveOperationIdentifier] = moveSession;
			}

			info.Response.NumberOfRemainingSubOperations = (ushort)sendParcel.GetToSendObjectCount();
			uint queryRetrieveOperationIdentifier = info.QueryRetrieveOperationIdentifier;

			EventHandler<BackgroundTaskTerminatedEventArgs> deleteHandler = delegate
				{
					lock (_moveSessionLock)
					{
						_moveSessionDictionary.Remove(queryRetrieveOperationIdentifier);
						task.Dispose();
					}
				};

			task.Terminated += deleteHandler;
			task.Run();

			info.Response.DimseStatus = moveSession.Status;
		}

		private void OnMoveScpProgressEvent(object sender, DicomEventArgs e)
		{
			InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);

			UpdateMoveProgress(info.QueryRetrieveOperationIdentifier, info);
		}

		#endregion

		#region DicomServer StoreScp Event Handlers

		private void OnStoreScpBeginEvent(object sender, DicomEventArgs e)
		{
			InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
		}

		private void OnStoreScpProgressEvent(object sender, DicomEventArgs e)
		{
			InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
		}

		private void OnStoreScpEndEvent(object sender, DicomEventArgs e)
		{
			InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);

			StoreScpReceivedFileInformation storedInformation = new StoreScpReceivedFileInformation();

			storedInformation.AETitle = info.CallingAETitle;
			storedInformation.FileName = info.FileName;

			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();

			try
			{
				client.Open();
				client.FileReceived(storedInformation);
				client.Close();
			}
			catch (Exception ex)
			{
				client.Abort();
				Platform.Log(LogLevel.Error, ex);
			}

			info.DimseStatus = (ushort)OffisDcm.STATUS_Success;
		}

		#endregion

		#region Store Scu Event Handlers

		private void OnStoreScuProgressEvent(object sender, DicomEventArgs e)
		{
			InteropStoreScuCallbackInfo info = new InteropStoreScuCallbackInfo(e.CallbackInfoPointer, false);

			T_DIMSE_C_StoreRQ request = info.Request;
			T_DIMSE_StoreProgress progress = info.Progress;

			if (progress.state != T_DIMSE_StoreProgressState.DIMSE_StoreEnd)
				return;

			StoreScuSentFileInformation sentFileInformation = new StoreScuSentFileInformation();

			sentFileInformation.ToAETitle = info.CalledAETitle;
			sentFileInformation.FileName = info.CurrentFile;

			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();

			try
			{
				client.Open();
				client.FileSent(sentFileInformation);
				client.Close();
			}
			catch (Exception ex)
			{
				client.Abort();
				Platform.Log(LogLevel.Error, ex);
			}
		}

		#endregion

		#region FindScp helper functions

		private QueryKey BuildQueryKey(DcmDataset requestIdentifiers)
		{
			OFCondition cond;
			QueryKey queryKey = new QueryKey();

			//See Dicom PS 3.4 Table C.6-5.  All required tags are supported for C-FIND except for Study Time.
			//Currently, Study Time can be returned, but not queried upon.
			string value;

			#region Required tags

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyDate, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyDate, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyTime, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyTime, value);
			
			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.AccessionNumber, out value);
			if (cond.good())
				queryKey.Add(DicomTags.AccessionNumber, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.PatientsName, out value);
			if (cond.good())
				queryKey.Add(DicomTags.PatientsName, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.PatientId, out value);
			if (cond.good())
				queryKey.Add(DicomTags.PatientId, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyID, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyId, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyInstanceUID, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyInstanceUid, value);

			#endregion

			#region Optional Tags

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyDescription, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyDescription, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.ModalitiesInStudy, out value);
			if (cond.good())
				queryKey.Add(DicomTags.ModalitiesInStudy, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.PatientsBirthDate, out value);
			if (cond.good())
				queryKey.Add(DicomTags.PatientsBirthDate, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.PatientsSex, out value);
			if (cond.good())
				queryKey.Add(DicomTags.PatientsSex, value);

			#endregion

			#region Conditional Tags

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.SpecificCharacterSet, out value);
            if (cond.good())
                queryKey.Add(DicomTags.SpecificCharacterSet, value);

			#endregion

			return queryKey;
		}

		private DicomQuerySession QueryDB(uint operationIdentifier, DcmDataset requestIdentifiers)
		{
			QueryKey key = null;
			ReadOnlyQueryResultCollection queryResults = null;
			DicomQuerySession querySession = null;
			ushort status = (ushort)OffisDcm.STATUS_Pending;

			try
			{
				string queryRetrieveLevel;
				OFCondition cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.QueryRetrieveLevel, out queryRetrieveLevel);
				if (queryRetrieveLevel == null || queryRetrieveLevel.Trim() != "STUDY")
				{
					status = (ushort) OffisDcm.STATUS_FIND_Failed_UnableToProcess;
				}
				else
				{
					// Query DB for results
					key = BuildQueryKey(requestIdentifiers);
					using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
					{
						queryResults = reader.StudyQuery(key);
					}
				}
			}
			catch (Exception exception)
			{
				Platform.Log(LogLevel.Error, exception);
				status = (ushort)OffisDcm.STATUS_FIND_Failed_UnableToProcess;
			}

			// Remember the query results for this session.  The DicomServer will call back to get query results
			lock (_querySessionLock)
			{
				querySession = new DicomQuerySession(key, queryResults);
				_querySessionDictionary[operationIdentifier] = querySession;
				_querySessionDictionary[operationIdentifier].DimseStatus = status;
			}

			return querySession;
		}

		private int GetNextQueryResult(uint operationIdentifier, DcmDataset responseIdentifiers)
		{
			QueryKey key;
			QueryResult result;

			lock (_querySessionLock)
			{
				// if key is not found, we return STATUS_Success anyway.  It means CFind has completed successfully.
				if (!_querySessionDictionary.ContainsKey(operationIdentifier))
					return OffisDcm.STATUS_Success;

				DicomQuerySession querySession = _querySessionDictionary[operationIdentifier];
				if (querySession.DimseStatus != OffisDcm.STATUS_Pending)
				{
					_querySessionDictionary.Remove(operationIdentifier);
					return querySession.DimseStatus;
				}
				
				if (querySession.CurrentIndex >= querySession.QueryResults.Count)
				{
					// If all the results have been retrieved, remove this query session from dictionary
					_querySessionDictionary.Remove(operationIdentifier);
					return OffisDcm.STATUS_Success;
				}

				key = querySession.QueryKey;
				// Otherwise, return the next query result based on the current query index
				result = querySession.QueryResults[querySession.CurrentIndex];
				++querySession.CurrentIndex;
			}

			//According to Dicom, we should include SpecificCharacterSet if any tags in the response could be affected by character replacement.  Otherwise, it should not be present.
			bool specificCharacterSetRequired = false;

			#region Required Tags

			responseIdentifiers.clear();

			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), result.StudyInstanceUid);
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "STUDY");
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.RetrieveAETitle), _dicomServer.AETitle);
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.InstanceAvailability), "ONLINE");
			
			if (key.ContainsTag(DicomTags.StudyDate))
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyDate), result.StudyDate);
			
			if (key.ContainsTag(DicomTags.StudyTime))
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyTime), result.StudyTime);

			if (key.ContainsTag(DicomTags.AccessionNumber))
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.AccessionNumber), result.AccessionNumber);

            // can have Specific Character Set effects
			if (key.ContainsTag(DicomTags.PatientsName))
			{
				specificCharacterSetRequired = true;
				byte[] encodedString = DicomImplementation.CharacterParser.Encode(result.PatientsName, result.SpecificCharacterSet);
				OffisDicomHelper.PutAndInsertRawStringIntoItem(responseIdentifiers, new DcmTag(Dcm.PatientsName), encodedString);
			}

			if (key.ContainsTag(DicomTags.PatientId))
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientId), result.PatientId);

			if (key.ContainsTag(DicomTags.StudyId))
			{
				specificCharacterSetRequired = true;
				byte[] encodedString = encodedString = DicomImplementation.CharacterParser.Encode(result.StudyID, result.SpecificCharacterSet);
				OffisDicomHelper.PutAndInsertRawStringIntoItem(responseIdentifiers, new DcmTag(Dcm.StudyID), encodedString);
			}

			#endregion

			#region Optional Tags

			if (key.ContainsTag(DicomTags.StudyDescription))
			{
				specificCharacterSetRequired = true;
				byte[] encodedString = encodedString = DicomImplementation.CharacterParser.Encode(result.StudyDescription, result.SpecificCharacterSet);
				OffisDicomHelper.PutAndInsertRawStringIntoItem(responseIdentifiers, new DcmTag(Dcm.StudyDescription), encodedString);
			}

			if (key.ContainsTag(DicomTags.ModalitiesInStudy))
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.ModalitiesInStudy), result.ModalitiesInStudy);

			if (key.ContainsTag(DicomTags.PatientsBirthDate))
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientsBirthDate), result.PatientsBirthDate);

			if (key.ContainsTag(DicomTags.PatientsSex))
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientsSex), result.PatientsSex);

			#endregion

			#region Conditional Tags

			if (specificCharacterSetRequired)
				responseIdentifiers.putAndInsertString(new DcmTag(Dcm.SpecificCharacterSet), result.SpecificCharacterSet);

			#endregion

			return OffisDcm.STATUS_Pending;
		}

		#endregion

		#region MoveScp helper functions

		private void UpdateMoveProgress(uint moveOperationIdentifier, InteropMoveScpCallbackInfo info)
		{
			DicomMoveSession session;

			lock (_moveSessionLock)
			{
				//Something bad happened in the 'begin' event.
				if (!_moveSessionDictionary.ContainsKey(moveOperationIdentifier))
				{
					info.Response.DimseStatus = (ushort) OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
					return;
				}
				
				session = _moveSessionDictionary[moveOperationIdentifier];

				if (session.Status != (ushort) OffisDcm.STATUS_Pending)
				{
					_moveSessionDictionary.Remove(info.QueryRetrieveOperationIdentifier);
					info.Response.DimseStatus = session.Status;
					return;
				}
			}
			// Keep the thread here and only return CMoveRSP when there's something different to report.
			while (session.Progress == session.Parcel.CurrentProgressStep && session.Parcel.IsActive())
			{
				Thread.Sleep(1000);
			};

			ParcelTransferState transferState;
			int currentProgressStep;
			int totalSteps;

			session.Parcel.GetSafeStats(out transferState, out totalSteps, out currentProgressStep);

			session.Progress = currentProgressStep;

			info.Response.NumberOfCompletedSubOperations = (ushort)currentProgressStep;
			info.Response.NumberOfRemainingSubOperations = (ushort)(totalSteps - currentProgressStep);

			switch (transferState)
			{
				case ParcelTransferState.Completed:
					info.Response.NumberOfCompletedSubOperations = (ushort)totalSteps;
					info.Response.NumberOfRemainingSubOperations = 0;
					info.Response.DimseStatus = (ushort)OffisDcm.STATUS_Success;
					break;
				case ParcelTransferState.Cancelled:
				case ParcelTransferState.CancelRequested:
					info.Response.NumberOfWarningSubOperations++;
					info.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Cancel_SubOperationsTerminatedDueToCancelIndication;
					info.ResponseIdentifiers.putAndInsertOFStringArray(new DcmTag(Dcm.FailedSOPInstanceUIDList),
					                                                   StringUtilities.Combine(session.Parcel.UnsentSopInstances, "\\"));
					break;
				case ParcelTransferState.Error:
				case ParcelTransferState.Unknown:
					info.Response.NumberOfFailedSubOperations++;
					info.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
					info.ResponseIdentifiers.putAndInsertOFStringArray(new DcmTag(Dcm.FailedSOPInstanceUIDList),
																	   StringUtilities.Combine(session.Parcel.UnsentSopInstances, "\\"));
					break;
				case ParcelTransferState.InProgress:
				case ParcelTransferState.Paused:
				case ParcelTransferState.PauseRequested:
				case ParcelTransferState.Pending:
				default:
					info.Response.DimseStatus = (ushort)OffisDcm.STATUS_Pending;
					break;
			}
		}

		#endregion

		private List<SendStudyInformation> ConvertToSendStudyInformation(IDictionary<IStudy, IList<ISopInstance>> sopInstancesByStudy)
		{
			List<SendStudyInformation> sendStudyInformation = new List<SendStudyInformation>();

			foreach (KeyValuePair<IStudy, IList<ISopInstance>> kvp in sopInstancesByStudy)
			{
				SendStudyInformation information = new SendStudyInformation();
				information.StudyInformation = new StudyInformation();

				Study study = kvp.Key as Study;
				if (study != null)
				{
					information.StudyInformation.PatientId = study.PatientId;
					information.StudyInformation.PatientsName = study.PatientsName;
					DateTime studyDate;
					DateParser.Parse(study.StudyDateRaw, out studyDate);
					information.StudyInformation.StudyDate = studyDate;
					information.StudyInformation.StudyDescription = study.StudyDescription;
				}

				information.StudyInformation.StudyInstanceUid = kvp.Key.GetStudyInstanceUid();

				sendStudyInformation.Add(information);
			}

			return sendStudyInformation;
		}

		#region IDicomServerService Members

		public void Send(AEInformation destinationAEInformation, IEnumerable<string> uids)
		{
			ApplicationEntity destinationAE;
			ApplicationEntity myApplicationEntity;

			lock (_serverConfigurationLock)
			{
				destinationAE = new ApplicationEntity(new HostName(destinationAEInformation.HostName), new AETitle(destinationAEInformation.AETitle), new ListeningPort(destinationAEInformation.Port));
				myApplicationEntity = new ApplicationEntity(new HostName(DicomServerSettings.Instance.HostName), new AETitle(DicomServerSettings.Instance.AETitle), new ListeningPort(DicomServerSettings.Instance.Port));
			}

			SendParcel parcel = new SendParcel(myApplicationEntity, destinationAE, "");
			foreach (string uid in uids)
				parcel.Include(new Uid(uid));

			BackgroundTaskContainer container = new BackgroundTaskContainer();

			BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext context)
			{
				LocalDataStoreServiceClient serviceClient = new LocalDataStoreServiceClient();

				try
				{
					List<SendStudyInformation> sendStudyInformation = ConvertToSendStudyInformation(parcel.SopInstancesByStudy);
					foreach (SendStudyInformation information in sendStudyInformation)
					{
						information.ToAETitle = destinationAE.AE;
						serviceClient.SendStarted(information);
					}

					serviceClient.Close();
				}
				catch (Exception e)
				{
					//not much we can do other than just log it.
					Platform.Log(LogLevel.Error, e);
					serviceClient.Abort();
				}

				try
				{
					parcel.Send();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);

					List<SendStudyInformation> incompleteStudyInformation = ConvertToSendStudyInformation(parcel.UnsentSopInstancesByStudy);

					serviceClient = new LocalDataStoreServiceClient();

					try
					{
						foreach (SendStudyInformation information in incompleteStudyInformation)
						{
							SendErrorInformation errorInformation = new SendErrorInformation();
							errorInformation.ToAETitle = destinationAE.AE;
							errorInformation.StudyInformation = information.StudyInformation;
							errorInformation.ErrorMessage = e.Message;
							serviceClient.SendError(errorInformation);
						}

						serviceClient.Close();
					}
					catch (Exception ex)
					{
						//not much we can do other than just log it.
						Platform.Log(LogLevel.Error, ex);
						serviceClient.Abort();
					}
				}
			}, false, container);

			container.Task = task;
			lock (_sendRetrieveTaskLock)
			{
				_sendRetrieveTasks.Add(container);
			}

			EventHandler<BackgroundTaskTerminatedEventArgs> deleteHandler = new EventHandler<BackgroundTaskTerminatedEventArgs>
				(delegate(object ignore, BackgroundTaskTerminatedEventArgs args)
				{
					lock (_sendRetrieveTaskLock)
					{
						_sendRetrieveTasks.Remove(container);
						task.Dispose();
					}
				});

			task.Terminated += deleteHandler;
			task.Run();
		}

		public void RetrieveStudies(AEInformation sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve)
		{
			ApplicationEntity myApplicationEntity;
			string saveDirectory;
			lock (_serverConfigurationLock)
			{
				myApplicationEntity = new ApplicationEntity(new HostName(DicomServerSettings.Instance.HostName),
														new AETitle(DicomServerSettings.Instance.AETitle), new ListeningPort(DicomServerSettings.Instance.Port));

				saveDirectory = DicomServerSettings.Instance.InterimStorageDirectory;
			}

			foreach (StudyInformation studyInformation in studiesToRetrieve)
			{
				StudyInformation retrieveStudyInformation = studyInformation;
				BackgroundTaskContainer container = new BackgroundTaskContainer();

				BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext context)
				{
					LocalDataStoreServiceClient serviceClient = new LocalDataStoreServiceClient();

					try
					{
						serviceClient.Open();
						RetrieveStudyInformation retrieveInformation = new RetrieveStudyInformation();
						retrieveInformation.FromAETitle = sourceAEInformation.AETitle;
						retrieveInformation.StudyInformation = retrieveStudyInformation;
						serviceClient.RetrieveStarted(retrieveInformation);
						serviceClient.Close();
					}
					catch (Exception ex)
					{
						//can't tell the Local Data Store service about the pending retrieve operation, not much we can do.
						Platform.Log(LogLevel.Error, ex);
						serviceClient.Abort();
					}

					try
					{
						DicomClient client = new DicomClient(myApplicationEntity);
						ApplicationEntity sourceAE = new ApplicationEntity(new HostName(sourceAEInformation.HostName),
														new AETitle(sourceAEInformation.AETitle), new ListeningPort(sourceAEInformation.Port));

						using (client)
						{
							client.RetrieveAsServiceClassUserOnly(sourceAE, new Uid(retrieveStudyInformation.StudyInstanceUid), saveDirectory);
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);

						serviceClient = new LocalDataStoreServiceClient();

						try
						{
							ReceiveErrorInformation errorInformation = new ReceiveErrorInformation();
							errorInformation.FromAETitle = sourceAEInformation.AETitle;
							errorInformation.StudyInformation = retrieveStudyInformation;
							errorInformation.ErrorMessage = e.Message;
							serviceClient.ReceiveError(errorInformation);
							serviceClient.Close();
						}
						catch (Exception ex)
						{
							//again, not much we can do.
							Platform.Log(LogLevel.Error, ex);
							serviceClient.Abort();
						}
					}

				}, false, container);

				container.Task = task;
				lock (_sendRetrieveTaskLock)
				{
					_sendRetrieveTasks.Add(container);
				}

				EventHandler<BackgroundTaskTerminatedEventArgs> deleteHandler = new EventHandler<BackgroundTaskTerminatedEventArgs>
					(delegate(object ignore, BackgroundTaskTerminatedEventArgs ignoreArgs)
					{
						lock (_sendRetrieveTaskLock)
						{
							_sendRetrieveTasks.Remove(container);
							task.Dispose();
						}
					});

				task.Terminated += deleteHandler;
				task.Run();
			}
		}

		public DicomServerConfiguration GetServerConfiguration()
		{
			lock (_serverConfigurationLock)
			{
				return new DicomServerConfiguration(DicomServerSettings.Instance.HostName,
												DicomServerSettings.Instance.AETitle,
												DicomServerSettings.Instance.Port,
												DicomServerSettings.Instance.InterimStorageDirectory);
			}
		}

		public void UpdateServerConfiguration(DicomServerConfiguration newConfiguration)
		{
			lock (_serverConfigurationLock)
			{
				lock (_restartServerLock)
				{
					DicomServerSettings.Instance.HostName = newConfiguration.HostName;
					DicomServerSettings.Instance.AETitle = newConfiguration.AETitle;
					DicomServerSettings.Instance.Port = newConfiguration.Port;
					DicomServerSettings.Instance.InterimStorageDirectory = newConfiguration.InterimStorageDirectory;
					DicomServerSettings.Save();

					if (_restartingServer)
						return;

					_restartingServer = true;
				}
			}

			WaitCallback del = delegate(object nothing)
			{
				StopServer();

				lock (_restartServerLock)
				{
					if (_restartingServer)
					{
						try
						{
							StartServer();
						}
						catch(Exception e)
						{
							Platform.Log(LogLevel.Error, new Exception("Failed to restart Dicom Server", e));
						}
						
						_restartingServer = false;
					}
				}
			};

			ThreadPool.QueueUserWorkItem(del);
		}

		#endregion
	}
}
