using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.DicomServices.Scu;
using ClearCanvas.ImageViewer.Services;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	#region Retrieve Service Definition

	internal class RetrieveStudiesRequest
	{
		public RetrieveStudiesRequest(AEInformation remoteAEInfo, IEnumerable<StudyInformation> studiesToRetrieve)
		{
			this.RemoteAEInfo = remoteAEInfo;
			this.StudiesToRetrieve = studiesToRetrieve;
		}

		public readonly AEInformation RemoteAEInfo;
		public readonly IEnumerable<StudyInformation> StudiesToRetrieve;
	}

	//TODO: later, remove IDicomServerService, extend this to support series and image retrievals, and add a callback interface.
	internal interface IRetrieveService
	{
		void RetrieveStudies(RetrieveStudiesRequest request);
	}

	#endregion

	internal class DicomRetrieveManager : IRetrieveService
	{
		public static readonly DicomRetrieveManager Instance = new DicomRetrieveManager();

		#region Private Fields

		private readonly object _syncLock = new object();
		private readonly List<RetrieveScu> _scus = new List<RetrieveScu>();
		private bool _active;

		#endregion

		private DicomRetrieveManager()
		{
			_active = false;
		}

		#region RetrieveScu class

		private class RetrieveScu : StudyRootMoveScu
		{
			#region Private Fields

			private readonly Thread _thread;
			private readonly IEnumerable<StudyInformation> _studiesToRetrieve;

			#endregion

			public RetrieveScu(string localAETitle, AEInformation remoteAEInfo, IEnumerable<StudyInformation> studiesToRetrieve)
				:base(localAETitle, remoteAEInfo.AETitle, remoteAEInfo.HostName, remoteAEInfo.Port, localAETitle)
			{
				Platform.CheckForEmptyString(localAETitle, "localAETitle");
				Platform.CheckForEmptyString(remoteAEInfo.AETitle, "remoteAEInfo.AETitle");
				Platform.CheckForEmptyString(remoteAEInfo.HostName, "remoteAEInfo.HostName");
				Platform.CheckForNullReference(studiesToRetrieve, "studiesToRetrieve");

				_studiesToRetrieve = studiesToRetrieve;

				_thread = new Thread(RetrieveInternal);
				//_thread.IsBackground = false;

				_thread.Name = String.Format("Retrieve from {0}/{1}:{2}", 
					remoteAEInfo.HostName, remoteAEInfo.AETitle, remoteAEInfo.Port);
			}

			public override void OnReceiveResponseMessage(ClearCanvas.Dicom.Network.DicomClient client, ClearCanvas.Dicom.Network.ClientAssociationParameters association, byte presentationID, ClearCanvas.Dicom.DicomMessage message)
			{
				base.OnReceiveResponseMessage(client, association, presentationID, message);

				if (message.Status.Status == DicomState.Cancel)
				{
					string msg = String.Format(
						"Remote server cancelled the operation corresponding to this C-MOVE-RQ ({0}: {1}).",
						RemoteAE, message.Status.Description);

					Platform.Log(LogLevel.Info, msg);
					OnRetrieveError(msg);
				}
				else if (message.Status.Status == DicomState.Warning)
				{
					string msg = String.Format("Remote server returned a warning status ({0}: {1}).",
						RemoteAE, message.Status.Description);

					Platform.Log(LogLevel.Warn, msg);
					OnRetrieveError(msg);
				}
				else if (message.Status.Status == DicomState.Failure)
				{
					string msg = String.Format("Remote server failed to process the C-MOVE request ({0}: {1}).",
						RemoteAE, message.Status.Description);

					Platform.Log(LogLevel.Error, msg);
					OnRetrieveError(msg);
				}
			}

			#region Private Methods

			private void RetrieveInternal()
			{
				try
				{
					OnBeginRetrieve();

					Move();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "MOVE operation failed: {0}:{1}:{2} -> {3}",
								base.RemoteAE, base.RemoteHost, base.RemotePort, base.ClientAETitle);

					OnRetrieveError(e.Message);
				}
				finally
				{
					Instance.OnRetrieveComplete(this);
				}
			}

			private void OnBeginRetrieve()
			{
				foreach (StudyInformation info in _studiesToRetrieve)
				{
					RetrieveStudyInformation retrieveInfo = new RetrieveStudyInformation();
					retrieveInfo.FromAETitle = base.RemoteAE;
					retrieveInfo.StudyInformation = info;
					LocalDataStoreEventPublisher.Instance.RetrieveStarted(retrieveInfo);
				}
			}

			private void OnRetrieveError(string message)
			{
				foreach (StudyInformation info in _studiesToRetrieve)
				{
					ReceiveErrorInformation receiveError = new ReceiveErrorInformation();
					receiveError.FromAETitle = base.RemoteAE;
					receiveError.StudyInformation = info;
					receiveError.ErrorMessage = message;
					LocalDataStoreEventPublisher.Instance.ReceiveError(receiveError);
				}
			}

			#endregion

			#region Public Methods

			public new void Cancel()
			{
				if (_thread.IsAlive)
				{
					base.Cancel();
					_thread.Join();
				}
			}

			public void Retrieve()
			{
				foreach (StudyInformation info in _studiesToRetrieve)
					AddStudyInstanceUid(info.StudyInstanceUid);

				//do this rather than use BeginSend b/c it uses thread pool threads which can be exhausted.
				_thread.Start();
			}

			#endregion
		}

		#endregion

		#region Private Methods

		private void OnRetrieveComplete(RetrieveScu scu)
		{
			lock (_syncLock)
			{
				_scus.Remove(scu);
				scu.Dispose();
			}
		}

		#endregion

		#region Public Methods

		public void Start()
		{
			lock (_syncLock)
			{
				_active = true;
			}
		}

		public void Stop()
		{
			List<RetrieveScu> scus;

			lock (_syncLock)
			{
				_active = false;
				scus = new List<RetrieveScu>(_scus);
			}

			scus.ForEach(delegate(RetrieveScu scu) { scu.Cancel(); });
		}
		
		#endregion

		#region IRetrieveService Members

		public void RetrieveStudies(RetrieveStudiesRequest request)
		{
			lock (_syncLock)
			{
				if (!_active)
					throw new InvalidOperationException("The Retrieve service is not running.");

				DicomServerConfiguration configuration = DicomServerManager.Instance.GetServerConfiguration();
				RetrieveScu scu = new RetrieveScu(configuration.AETitle, request.RemoteAEInfo, request.StudiesToRetrieve);

				_scus.Add(scu);
				scu.Retrieve();
			}
		}

		#endregion
	}
}
