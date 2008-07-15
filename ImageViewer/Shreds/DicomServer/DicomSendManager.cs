using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.DicomServices.Scu;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Dicom.Network;

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

	internal class SendInstancesRequest : SendRequest
	{
		public SendInstancesRequest(
			AEInformation destinationAEInformation, 
			IEnumerable<string> sendUids, 
			SendOperationProgressCallback callback)
			: base(destinationAEInformation, callback)
		{
			SendUids = sendUids;
		}

		public readonly IEnumerable<string> SendUids;
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

		//TODO: later, remove this.  We only need it for compatibility with the existing IDicomServerService interface.
		Guid SendInstances(SendInstancesRequest request);

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
				if (_studies.ContainsKey(study.GetStudyInstanceUid()))
					return;
				
				Study s = (Study) study;
				SendStudyInformation info = new SendStudyInformation();
				info.ToAETitle = RemoteAE;

				info.StudyInformation = new StudyInformation();
				info.StudyInformation.PatientId = s.PatientId;
				info.StudyInformation.PatientsName = s.PatientsName;
				info.StudyInformation.StudyDate = s.StudyDate;
				info.StudyInformation.StudyDescription = s.StudyDescription;
				info.StudyInformation.StudyInstanceUid = s.StudyInstanceUid;

				_studies[study.GetStudyInstanceUid()] = info;
			}

			private void SendInternal()
			{
				try
				{
					OnBeginSend();

					base.Send();
				}
				catch(Exception e)
				{
					Platform.Log(LogLevel.Error, e, "An error occurred while processing the store operation.");
					OnSendError(e.Message);
				}
				finally
				{
					Instance.OnSendComplete(this);
				}
			}

			public override void OnReceiveResponseMessage(ClearCanvas.Dicom.Network.DicomClient client, ClearCanvas.Dicom.Network.ClientAssociationParameters association, byte presentationID, ClearCanvas.Dicom.DicomMessage message)
			{
				base.OnReceiveResponseMessage(client, association, presentationID, message);

				if (message.Status.Status == DicomState.Cancel)
				{
					string msg = String.Format(
						"Remote server cancelled the C-STORE operation ({0}: {1}).",
						RemoteAE, message.Status.Description);

					Platform.Log(LogLevel.Info, msg);
					OnSendError(msg);
				}
				else if (message.Status.Status == DicomState.Warning)
				{
					string msg = String.Format("Remote server returned a warning status ({0}: {1}).",
						RemoteAE, message.Status.Description);

					Platform.Log(LogLevel.Warn, msg);
					OnSendError(msg);
				}
				else if (message.Status.Status == DicomState.Failure)
				{
					string msg = String.Format("Remote server failed to process the C-STORE request ({0}: {1}).",
						RemoteAE, message.Status.Description);

					Platform.Log(LogLevel.Error, msg);
					OnSendError(msg);
				}
			}

			protected override void OnImageStoreCompleted(StorageInstance storageInstance)
			{
				base.OnImageStoreCompleted(storageInstance);

				if (storageInstance.SendStatus == DicomStatuses.Success)
				{
					StoreScuSentFileInformation info = new StoreScuSentFileInformation();
					info.ToAETitle = RemoteAE;
					info.FileName = storageInstance.Filename;

					LocalDataStoreEventPublisher.Instance.FileSent(info);
				}
				else
				{
					string msg = String.Format("Error attempting to send file {0} ({1}:{2}).",
						storageInstance.Filename, 
						RemoteAE,
						storageInstance.ExtendedFailureDescription);

					Platform.Log(LogLevel.Error, msg);
					OnSendError(msg);
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

		private static void ValidateParentStudy(ISeries series, string studyInstanceUid)
		{
			if (series.GetParentStudy().GetStudyInstanceUid() != studyInstanceUid)
			{
				string message = String.Format("The given series exists in the database ({0}), " +
											   "but the provided study uid differs from the one in the local database (local = {1}, provided = {2}).",
											   series.GetSeriesInstanceUid(), series.GetParentStudy().GetStudyInstanceUid(), studyInstanceUid);

				throw new ArgumentException(message);
			}
		}

		private static void ValidateParentSeriesAndStudy(ISopInstance sop, string seriesInstanceUid, string studyInstanceUid)
		{
			if (sop.GetParentSeries().GetSeriesInstanceUid() != seriesInstanceUid)
			{
				string message = String.Format("The given sop exists in the database ({0}), " +
											   "but the provided series uid differs from the one in the local database (local = {1}, provided = {2}).",
											   sop.GetSopInstanceUid(), sop.GetParentSeries().GetSeriesInstanceUid(), seriesInstanceUid);

				throw new ArgumentException(message);
			}

			if (sop.GetParentSeries().GetParentStudy().GetStudyInstanceUid() != studyInstanceUid)
			{
				string message = String.Format("The given sop exists in the database ({0}), " +
											   "but the provided study uid differs from the one in the local database (local = {1}, provided = {2}).",
											   sop.GetSopInstanceUid(), sop.GetParentSeries().GetParentStudy().GetStudyInstanceUid(), studyInstanceUid);

				throw new ArgumentException(message);
			}
		}

		private static IEnumerable<ISopInstance> GetStudySopInstances(IEnumerable<string> studyInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (string studyInstanceUid in studyInstanceUids)
				{
					IStudy study = reader.GetStudy(studyInstanceUid);
					if (study == null)
					{
						string message = String.Format("The specified study does not exist in the database (uid = {0}).", studyInstanceUid);
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
				foreach (string seriesInstanceUid in seriesInstanceUids)
				{
					ISeries series = reader.GetSeries(seriesInstanceUid);
					if (series == null)
					{
						string message = String.Format("The specified series does not exist in the database (uid = {0}).", seriesInstanceUid);
						throw new ArgumentException(message);
					}

					ValidateParentStudy(series, studyInstanceUid);
					foreach (ISopInstance sop in series.GetSopInstances())
						yield return sop;
				}
			}
		}

		private static IEnumerable<ISopInstance> GetSopInstances(string studyInstanceUid, string seriesInstanceUid, IEnumerable<string> sopInstanceUids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (string sopInstanceUid in sopInstanceUids)
				{
					ISopInstance sop = reader.GetSopInstance(sopInstanceUid);
					if (sop == null)
					{
						string message = String.Format("The specified sop instance does not exist in the database (uid = {0}).", sopInstanceUid);
						throw new ArgumentException(message);
					}

					ValidateParentSeriesAndStudy(sop, seriesInstanceUid, studyInstanceUid);
					yield return sop;
				}
			}
		}

		private static IEnumerable<ISopInstance> GetSopInstancesFromUnknownLevels(IEnumerable<string> uids)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				foreach (string uid in uids)
				{
					if (reader.StudyExists(uid))
					{
						IStudy study = reader.GetStudy(uid);
						foreach (ISopInstance sop in study.GetSopInstances())
							yield return sop;
					}
					else if (reader.SeriesExists(uid))
					{
						ISeries series = reader.GetSeries(uid);
						foreach (ISopInstance sop in series.GetSopInstances())
							yield return sop;
					}
					else if (reader.SopInstanceExists(uid))
					{
						ISopInstance sop = reader.GetSopInstance(uid);
						yield return sop;
					}
					else
					{
						string message = String.Format("The specified uid does not exist in the database at any level (uid = {0}).", uid);
						throw new ArgumentException(message);
					}
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

		public Guid SendInstances(SendInstancesRequest request)
		{
			return Send(request.DestinationAEInformation, 
				GetSopInstancesFromUnknownLevels(request.SendUids), request.Callback);
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
