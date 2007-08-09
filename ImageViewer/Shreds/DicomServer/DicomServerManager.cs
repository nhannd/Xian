using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;
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

		private volatile ClearCanvas.Dicom.Network.DicomServer _dicomServer;

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

				Platform.Log(LogLevel.Info, "DICOM Server stopped");
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

				_dicomServer = new ClearCanvas.Dicom.Network.DicomServer(myApplicationEntity, DicomServerSettings.Instance.InterimStorageDirectory);
			}

			try
			{
				_dicomServer.Start();
				Platform.Log(LogLevel.Info, "DICOM Server started");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, new Exception("Failed to start Dicom Server", e));
			}
		}

		#region Properties

		#endregion

		#region DicomServer FindScp Event Handlers

		private void OnFindScpEvent(object sender, DicomEventArgs e)
		{
			InteropFindScpCallbackInfo info = new InteropFindScpCallbackInfo(e.CallbackInfoPointer, false);
			if (info == null)
				return;

			info.Response.DimseStatus = (ushort)QueryDB(info.QueryRetrieveOperationIdentifier, info.RequestIdentifiers);
		}

		private void OnFindScpProgressEvent(object sender, DicomEventArgs e)
		{
			InteropFindScpCallbackInfo info = new InteropFindScpCallbackInfo(e.CallbackInfoPointer, false);
			if (info == null)
				return;

			info.Response.DimseStatus = (ushort)GetNextQueryResult(info.QueryRetrieveOperationIdentifier, info.ResponseIdentifiers);
		}

		#endregion

		#region DicomServer MoveScp Event Handlers

		private void OnMoveScpBeginEvent(object sender, DicomEventArgs e)
		{
			InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);
			if (info == null)
				return;

			lock (_moveSessionLock)
			{
				if (_moveSessionDictionary.ContainsKey(info.QueryRetrieveOperationIdentifier))
				{
					//this should never happen.
					info.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
					return;
				}
			}

			// Start the Query
			info.Response.DimseStatus = (ushort)QueryDB(info.QueryRetrieveOperationIdentifier, info.RequestIdentifiers);

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
				info.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_MoveDestinationUnknown;
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
					info.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
					return;
				}

				sendParcel.Include(new Uid(studyInstanceUID));
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
				_moveSessionDictionary[info.QueryRetrieveOperationIdentifier] = new DicomMoveSession(sendParcel, task);
			}

			info.Response.NumberOfRemainingSubOperations = (ushort)sendParcel.GetToSendObjectCount();

			EventHandler<BackgroundTaskTerminatedEventArgs> deleteHandler = new EventHandler<BackgroundTaskTerminatedEventArgs>
				(delegate(object ignore, BackgroundTaskTerminatedEventArgs ignoreArgs)
				{
					lock (_moveSessionLock)
					{
						_moveSessionDictionary.Remove(info.QueryRetrieveOperationIdentifier);
						task.Dispose();
					}
				});

			task.Terminated += deleteHandler;
			task.Run();

			info.Response.DimseStatus = (ushort)OffisDcm.STATUS_Pending;
		}

		private void OnMoveScpProgressEvent(object sender, DicomEventArgs e)
		{
			InteropMoveScpCallbackInfo info = new InteropMoveScpCallbackInfo(e.CallbackInfoPointer, false);
			if (info == null)
				return;

			info.Response.DimseStatus = (ushort)UpdateMoveProgress(info.QueryRetrieveOperationIdentifier, info.Response);
		}

		#endregion

		#region DicomServer StoreScp Event Handlers

		private void OnStoreScpBeginEvent(object sender, DicomEventArgs e)
		{
			InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
			if (info == null)
				return;
		}

		private void OnStoreScpProgressEvent(object sender, DicomEventArgs e)
		{
			InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
			if (info == null)
				return;
		}

		private void OnStoreScpEndEvent(object sender, DicomEventArgs e)
		{
			InteropStoreScpCallbackInfo info = new InteropStoreScpCallbackInfo(e.CallbackInfoPointer, false);
			if (info == null)
				return;

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
			if (info == null)
				return;

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

			// TODO: Edit these when we need to expand the support of search parameters
			string value;
			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.PatientId, out value);
			if (cond.good())
				queryKey.Add(DicomTags.PatientID, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.AccessionNumber, out value);
			if (cond.good())
				queryKey.Add(DicomTags.AccessionNumber, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.PatientsName, out value);
			if (cond.good())
				queryKey.Add(DicomTags.PatientsName, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyDate, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyDate, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyDescription, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyDescription, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.ModalitiesInStudy, out value);
			if (cond.good())
				queryKey.Add(DicomTags.ModalitiesinStudy, value);

			cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.StudyInstanceUID, out value);
			if (cond.good())
				queryKey.Add(DicomTags.StudyInstanceUID, value);

            cond = OffisDicomHelper.TryFindAndGetOFString(requestIdentifiers, Dcm.SpecificCharacterSet, out value);
            if (cond.good())
                queryKey.Add(DicomTags.SpecificCharacterSet, value);

			return queryKey;
		}

		private int QueryDB(uint operationIdentifier, DcmDataset requestIdentifiers)
		{
			try
			{
				// Query DB for results
				ReadOnlyQueryResultCollection queryResults = DataAccessLayer.GetIDataStoreReader().StudyQuery(BuildQueryKey(requestIdentifiers));
				if (queryResults.Count == 0)
					return OffisDcm.STATUS_Success;

				// Remember the query results for this session.  The DicomServer will call back to get query results
				lock (_querySessionLock)
				{
					_querySessionDictionary[operationIdentifier] = new DicomQuerySession(queryResults);
				}
			}
			catch (Exception exception)
			{
				Platform.Log(LogLevel.Error, exception);
				return OffisDcm.STATUS_FIND_Failed_UnableToProcess;
			}

			return OffisDcm.STATUS_Pending;
		}

		private int GetNextQueryResult(uint operationIdentifier, DcmDataset responseIdentifiers)
		{
			QueryResult result;
			DicomQuerySession querySession;

			try
			{
				lock (_querySessionLock)
				{
					querySession = _querySessionDictionary[operationIdentifier];
					if (querySession.CurrentIndex >= querySession.QueryResults.Count)
					{
						// If all the results had been retrieved, remove this query session from dictionary
						_querySessionDictionary.Remove(operationIdentifier);
						return OffisDcm.STATUS_Success;
					}
				}

				// Otherwise, return the next query result based on the current query index
				result = querySession.QueryResults[querySession.CurrentIndex];
				querySession.CurrentIndex++;
			}
			catch (KeyNotFoundException)
			{
				// if key is not found, we return STATUS_Success anyway.  It means CFind has completed successfully
				return OffisDcm.STATUS_Success;
			}
			catch (Exception exception)
			{
				Platform.Log(LogLevel.Error, exception);
				return OffisDcm.STATUS_FIND_Failed_UnableToProcess;
			}

			// Edit these when we need to expand the list of supported return tags
			responseIdentifiers.clear();
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientId), result.PatientId);

            // strings that can have Specific Character Set effects
            byte[] encodedString = DicomImplementation.CharacterParser.Encode(result.PatientsName, result.SpecificCharacterSet);
            ClearCanvas.Dicom.Network.OffisDicomHelper.PutAndInsertRawStringIntoItem(responseIdentifiers, new DcmTag(Dcm.PatientsName), encodedString);
            encodedString = DicomImplementation.CharacterParser.Encode(result.StudyDescription, result.SpecificCharacterSet);
            ClearCanvas.Dicom.Network.OffisDicomHelper.PutAndInsertRawStringIntoItem(responseIdentifiers, new DcmTag(Dcm.StudyDescription), encodedString);

			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyDate), result.StudyDate);
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyTime), result.StudyTime);

			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.ModalitiesInStudy), result.ModalitiesInStudy);
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.AccessionNumber), result.AccessionNumber);
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), result.StudyInstanceUid);
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "STUDY");
			responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), result.StudyInstanceUid);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.SpecificCharacterSet), result.SpecificCharacterSet);

			return OffisDcm.STATUS_Pending;
		}

		#endregion

		#region MoveScp helper functions

		private int UpdateMoveProgress(uint moveOperationIdentifier, T_DIMSE_C_MoveRSP response)
		{
			int status = OffisDcm.STATUS_Pending;
			DicomMoveSession session;

			lock (_moveSessionLock)
			{
				if (_moveSessionDictionary.ContainsKey(moveOperationIdentifier) == false)
					return OffisDcm.STATUS_MOVE_Failed_UnableToProcess;

				session = _moveSessionDictionary[moveOperationIdentifier];
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

			response.NumberOfCompletedSubOperations = (ushort)currentProgressStep;
			response.NumberOfRemainingSubOperations = (ushort)(totalSteps - currentProgressStep);

			switch (transferState)
			{
				case ParcelTransferState.Completed:
					response.NumberOfCompletedSubOperations = (ushort)totalSteps;
					response.NumberOfRemainingSubOperations = 0;
					status = OffisDcm.STATUS_Success;
					break;
				case ParcelTransferState.Cancelled:
				case ParcelTransferState.CancelRequested:
					response.NumberOfWarningSubOperations++;
					status = OffisDcm.STATUS_MOVE_Cancel_SubOperationsTerminatedDueToCancelIndication;
					break;
				case ParcelTransferState.Error:
				case ParcelTransferState.Unknown:
					response.NumberOfFailedSubOperations++;
					status = OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
					break;
				case ParcelTransferState.InProgress:
				case ParcelTransferState.Paused:
				case ParcelTransferState.PauseRequested:
				case ParcelTransferState.Pending:
				default:
					status = OffisDcm.STATUS_Pending;
					break;
			}

#if DEBUG 
			Console.WriteLine("MOVE - Completed: {0} Remaining: {1}", response.NumberOfCompletedSubOperations, response.NumberOfRemainingSubOperations);
#endif
			return status;
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
						StartServer();
						_restartingServer = false;
					}
				}
			};

			ThreadPool.QueueUserWorkItem(del);
		}

		#endregion
	}
}
