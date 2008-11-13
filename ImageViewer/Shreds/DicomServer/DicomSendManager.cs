using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	#region Send Service Definition

	#region Send Request classes

	internal abstract class SendRequest
	{
		public SendRequest(AEInformation destinationAEInformation, 
			SendOperationProgressCallback callback)
		{
			DestinationAEInformation = destinationAEInformation;
			Callback = callback;
		}

		public readonly AEInformation DestinationAEInformation;
		public readonly SendOperationProgressCallback Callback;
	}

	internal class SendStudiesRequest : SendRequest
	{
		public SendStudiesRequest(AEInformation destinationAEInformation,
			IEnumerable<string> studyInstanceUids, 
			SendOperationProgressCallback callback)
			: base(destinationAEInformation, callback)
		{
			StudyInstanceUids = studyInstanceUids;
		}

		public readonly IEnumerable<string> StudyInstanceUids;
	}

	internal class SendSeriesRequest : SendRequest
	{
		public SendSeriesRequest(AEInformation destinationAEInformation,
			string studyInstanceUid,
			IEnumerable<string> seriesInstanceUids,
			SendOperationProgressCallback callback)
			: base(destinationAEInformation, callback)
		{
			StudyInstanceUid = studyInstanceUid;
			SeriesInstanceUids = seriesInstanceUids;
		}

		public readonly string StudyInstanceUid;
		public readonly IEnumerable<string> SeriesInstanceUids;
	}

	internal class SendSopInstancesRequest : SendRequest
	{
		public SendSopInstancesRequest(
			AEInformation destinationAEInformation, 
			string studyInstanceUid,
			string seriesInstanceUid,
			IEnumerable<string> sopInstanceUids, 
			SendOperationProgressCallback callback)
			: base(destinationAEInformation, callback)
		{
			StudyInstanceUid = studyInstanceUid;
			SeriesInstanceUid = seriesInstanceUid;
			SopInstanceUids = sopInstanceUids;
		}

		public readonly string StudyInstanceUid;
		public readonly string SeriesInstanceUid;
		public readonly IEnumerable<string> SopInstanceUids;
	}

	#endregion
	#endregion

	internal delegate void SendOperationProgressCallback(ISendOperation operation);

	internal interface ISendOperation
	{
		Guid Identifier { get; }
		bool Canceled { get; }

		int RemainingSubOperations { get; }
		int FailureSubOperations { get; }
		int SuccessSubOperations { get; }
		int WarningSubOperations { get; }

		ICollection<StorageInstance> StorageInstances { get; }
	}

	//TODO: Later, remove IDicomServerService and replace with something like this.
	internal interface ISendService
	{
		Guid SendStudies(SendStudiesRequest request);
		Guid SendSeries(SendSeriesRequest request);
		Guid SendSopInstances(SendSopInstancesRequest request);

		void Cancel(Guid operationIdentifier);
	}

	internal class DicomSendManager : ISendService
	{
		public static readonly DicomSendManager Instance = new DicomSendManager();

		#region Private Fields

		private readonly object _syncLock = new object();
		private readonly List<SendScu> _scus = new List<SendScu>();
		private bool _active;

		#endregion

		private DicomSendManager()
		{
			_active = false;
		}

		#region SendScu class

		private class SendScu : StorageScu, ISendOperation
		{
			#region Private Fields

			private readonly Guid _identifier;
			private readonly Dictionary<string, SendStudyInformation> _studies;
			private readonly Thread _thread;

			private readonly SendOperationProgressCallback _callback;

			#endregion

			public SendScu(string localAETitle, AEInformation destinationAEInfo, IEnumerable<ISopInstance> instancesToSend, SendOperationProgressCallback callback)
				: base(localAETitle, destinationAEInfo.AETitle, destinationAEInfo.HostName, destinationAEInfo.Port)
			{
				Platform.CheckForEmptyString(localAETitle, "localAETitle");
				Platform.CheckForEmptyString(destinationAEInfo.AETitle, "destinationAEInfo.AETitle");
				Platform.CheckForEmptyString(destinationAEInfo.HostName, "destinationAEInfo.HostName");
				Platform.CheckForNullReference(instancesToSend, "instancesToSend");

				_studies = new Dictionary<string, SendStudyInformation>();
				Initialize(instancesToSend);

				_identifier = Guid.NewGuid();

				_thread = new Thread(SendInternal);
				_thread.Name = String.Format("Send to {0}/{1}:{2}", base.RemoteHost, base.RemoteAE, base.RemotePort);

				_callback = callback;
			}

			private void Initialize(IEnumerable<ISopInstance> instancesToSend)
			{
				foreach (ISopInstance sop in instancesToSend)
				{
					AddStudy(sop.GetParentSeries().GetParentStudy());
					AddStorageInstance(new StorageInstance(sop.GetLocationUri().LocalDiskPath));
				}
			}

			private void AddStudy(IStudy study)
			{
				if (_studies.ContainsKey(study.StudyInstanceUid))
					return;
				
				SendStudyInformation info = new SendStudyInformation();
				info.ToAETitle = RemoteAE;

				info.StudyInformation = new StudyInformation();
				info.StudyInformation.PatientId = study.PatientId;
				info.StudyInformation.PatientsName = study.PatientsName;
				info.StudyInformation.StudyDate = DateParser.Parse(study.StudyDate);
				info.StudyInformation.StudyDescription = study.StudyDescription;
				info.StudyInformation.StudyInstanceUid = study.StudyInstanceUid;

				_studies[study.StudyInstanceUid] = info;
			}

			private void SendInternal()
			{
				try
				{
					OnBeginSend();

					base.Send();

					Join(new TimeSpan(0, 0, 0, 0, 1000));

					if (base.Status == ScuOperationStatus.Canceled)
					{
						OnSendError(String.Format("The C-STORE operation has been cancelled ({0}).", RemoteAE));
					}
					else if (base.Status == ScuOperationStatus.ConnectFailed)
					{
						OnSendError(String.Format("Unable to connect to remote server ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.Failed)
					{
						OnSendError(String.Format("The C-STORE operation failed ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.TimeoutExpired)
					{
						OnSendError(String.Format("The connection timeout has expired ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e, "An error occurred while processing the C-STORE operation.");
					string message = String.Format("An unexpected error occurred while processing the C-STORE operation ({0}).",
					                               e.Message);
					OnSendError(message);
				}
				finally
				{
					Instance.OnSendComplete(this);
				}
			}

			private void OnFileSent(StorageInstance storageInstance)
			{
				StoreScuSentFileInformation info = new StoreScuSentFileInformation();
				info.ToAETitle = RemoteAE;
				info.FileName = storageInstance.Filename;
				LocalDataStoreEventPublisher.Instance.FileSent(info);
			}

			protected override void OnImageStoreCompleted(StorageInstance storageInstance)
			{
				base.OnImageStoreCompleted(storageInstance);

				if (storageInstance.SendStatus.Status == DicomState.Success)
				{
					OnFileSent(storageInstance);
				}
				else if (storageInstance.SendStatus.Status != DicomState.Pending)
				{
					string severity = "Error";
					if (storageInstance.SendStatus.Status == DicomState.Warning)
					{
						severity = "Warning";
						OnFileSent(storageInstance);
					}

					string description = storageInstance.ExtendedFailureDescription;
					if (String.IsNullOrEmpty(description))
						description = storageInstance.SendStatus.ToString();

					string msg = String.Format("{0} encountered while sending file {1} ({2}: {3}).",
						severity, storageInstance.Filename, RemoteAE, description);

					Platform.Log(LogLevel.Error, msg);

					OnSendError(String.Format("{0} encountered while sending file ({1}: {2}).",
											  severity, RemoteAE, description));
				}

				if (_callback != null)
					_callback(this);
			}

			private void OnBeginSend()
			{
				//later, we could queue it up to limit the number of active scus.
				foreach (SendStudyInformation info in _studies.Values)
					LocalDataStoreEventPublisher.Instance.SendStarted(info);
			}

			private void OnSendError(string message)
			{
				foreach (SendStudyInformation info in _studies.Values)
				{
					SendErrorInformation error = new SendErrorInformation();
					error.ToAETitle = info.ToAETitle;
					error.StudyInformation = info.StudyInformation;
					error.ErrorMessage = message;
					LocalDataStoreEventPublisher.Instance.SendError(error);
				}
			}

			#region IStorageScu Members

			public Guid Identifier
			{
				get { return _identifier; }	
			}

			public ICollection<StorageInstance> StorageInstances
			{
				get { return StorageInstanceList; }
			}

			#endregion

			public new void Send()
			{
				//do this rather than use BeginSend b/c it uses thread pool threads which can be exhausted.
				_thread.Start();
			}

			public void Cancel(bool wait)
			{
				Cancel();
				if (wait)
					_thread.Join();
			}
		}

		#endregion

		#region Private Methods

		private static IEnumerable<ISopInstance> GetStudySopInstances(IEnumerable<string> studyInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (string studyInstanceUid in studyInstanceUids)
				{
					IStudy study = reader.GetStudy(studyInstanceUid);
					if (study == null)
					{
						string message = String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
						throw new ArgumentException(message);
					}

					foreach (ISopInstance sop in study.GetSopInstances())
						yield return sop;
				}
			}
		}

		private static IEnumerable<ISopInstance> GetSeriesSopInstances(string studyInstanceUid, IEnumerable<string> seriesInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(studyInstanceUid);
				if (study == null)
				{
					string message = String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
					throw new ArgumentException(message);
				}

				foreach (string seriesInstanceUid in seriesInstanceUids)
				{
					ISeries series = CollectionUtils.SelectFirst(study.GetSeries(),
										delegate(ISeries test) { return test.SeriesInstanceUid == seriesInstanceUid; });

					if (series == null)
					{
						string message = String.Format("The specified series does not exist in the data store (study = {0}, series = {1}).", studyInstanceUid, seriesInstanceUid);
						throw new ArgumentException(message);
					}

					foreach (ISopInstance sop in series.GetSopInstances())
						yield return sop;
				}
			}
		}

		private static IEnumerable<ISopInstance> GetSopInstances(string studyInstanceUid, string seriesInstanceUid, IEnumerable<string> sopInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(studyInstanceUid);
				if (study == null)
				{
					string message = String.Format("The specified study does not exist in the data store (uid = {0}).", studyInstanceUid);
					throw new ArgumentException(message);
				}

				ISeries series = CollectionUtils.SelectFirst(study.GetSeries(),
									delegate(ISeries test) { return test.SeriesInstanceUid == seriesInstanceUid; });

				if (series == null)
				{
					string message = String.Format("The specified series does not exist in the data store (study = {0}, series = {1}).", studyInstanceUid, seriesInstanceUid);
					throw new ArgumentException(message);
				}
					
				foreach (string sopInstanceUid in sopInstanceUids)
				{
					ISopInstance sop = CollectionUtils.SelectFirst(series.GetSopInstances(),
										delegate(ISopInstance test) { return test.SopInstanceUid == sopInstanceUid; });

					if (sop == null)
					{
						string message = String.Format("The specified sop instance does not exist in the data store (study = {0}, series = {1}, sop = {2}).", studyInstanceUid, seriesInstanceUid, sopInstanceUid);
						throw new ArgumentException(message);
					}

					yield return sop;
				}
			}
		}

		private Guid Send(AEInformation destinationAEInformation, IEnumerable<ISopInstance> instancesToSend, SendOperationProgressCallback callback)
		{
			lock (_syncLock)
			{
				if (!_active)
					throw new InvalidOperationException("The Dicom Send service is not active.");

				DicomServerConfiguration configuration = DicomServerManager.Instance.GetServerConfiguration();
				SendScu scu = new SendScu(configuration.AETitle, destinationAEInformation, instancesToSend, callback);
				_scus.Add(scu);
				scu.Send();
				return scu.Identifier;
			}
		}

		private void OnSendComplete(SendScu sendScu)
		{
			lock (_syncLock)
			{
				_scus.Remove(sendScu);
				sendScu.Dispose();
			}
		}

		#endregion

		#region public Methods

		public void Start()
		{
			lock(_syncLock)
			{
				_active = true;
			}
		}

		public void Stop()
		{
			List<SendScu> scus;
			lock (_syncLock)
			{
				_active = false;
				scus = new List<SendScu>(_scus);
			}

			scus.ForEach(delegate(SendScu scu) { scu.Cancel(true); });
		}

		#endregion

		#region ISendService Members

		public Guid SendStudies(SendStudiesRequest request)
		{
			return Send(request.DestinationAEInformation, 
				GetStudySopInstances(request.StudyInstanceUids), request.Callback);
		}

		public Guid SendSeries(SendSeriesRequest request)
		{
			return Send(request.DestinationAEInformation, 
				GetSeriesSopInstances(request.StudyInstanceUid, request.SeriesInstanceUids), request.Callback);
		}

		public Guid SendSopInstances(SendSopInstancesRequest request)
		{
			return Send(request.DestinationAEInformation,
				GetSopInstances(request.StudyInstanceUid, request.SeriesInstanceUid, request.SopInstanceUids), request.Callback);
		}

		public void Cancel(Guid operationIdentifier)
		{
			lock (_syncLock)
			{
				SendScu scu = _scus.Find(delegate(SendScu test) { return test.Identifier == operationIdentifier; });
				if (scu != null)
					scu.Cancel(false);
			}
		}

		#endregion
	}
}
