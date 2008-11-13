using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
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
				_thread.Name = String.Format("Retrieve from {0}/{1}:{2}", 
					remoteAEInfo.HostName, remoteAEInfo.AETitle, remoteAEInfo.Port);
			}

			public override void OnReceiveResponseMessage(ClearCanvas.Dicom.Network.DicomClient client, ClearCanvas.Dicom.Network.ClientAssociationParameters association, byte presentationID, ClearCanvas.Dicom.DicomMessage message)
			{
				base.OnReceiveResponseMessage(client, association, presentationID, message);

				if (message.Status.Status == DicomState.Warning)
				{
					string msg = String.Format("Remote server returned a warning status ({0}: {1}).",
						RemoteAE, message.Status.Description);
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

					Join(new TimeSpan(0, 0, 0, 0, 1000));

					if (base.Status == ScuOperationStatus.Canceled)
					{
						OnRetrieveError(String.Format("The C-MOVE operation was cancelled ({0}).", RemoteAE));
					}
					else if (base.Status == ScuOperationStatus.ConnectFailed)
					{
						OnRetrieveError(String.Format("Unable to connect to remote server ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.Failed)
					{
						OnRetrieveError(String.Format("The C-MOVE operation failed ({0}: {1}).",
							RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
					else if (base.Status == ScuOperationStatus.TimeoutExpired)
					{
						//When the scu hasn't received a progress update for the period of the timeout,
						//we end up showing this message.  Some SCPs won't even send progress, so you
						//would see this message constantly.

						//OnRetrieveError(String.Format("The connection timeout has expired ({0}: {1}).",
						//    RemoteAE, base.FailureDescription ?? "no failure description provided"));
					}
				}
				catch (Exception e)
				{
					string message = String.Format("C-MOVE operation failed: {0}:{1}:{2} -> {3}",
										base.RemoteAE, base.RemoteHost, base.RemotePort, base.ClientAETitle);
					Platform.Log(LogLevel.Error, e, message);

					OnRetrieveError(String.Format("C-MOVE operation failed: {0}:{1}:{2} -> {3}; {4}",
										base.RemoteAE, base.RemoteHost, base.RemotePort, base.ClientAETitle, e.Message));
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
				foreach (StudyInformation study in request.StudiesToRetrieve)
				{
					//Some servers seems to have a problem with C-MOVE-RQs that have more than one study uid,
					//so we'll just do them one at a time.
					List<StudyInformation> retrieveStudy = new List<StudyInformation>();
					retrieveStudy.Add(study);
					RetrieveScu scu = new RetrieveScu(configuration.AETitle, request.RemoteAEInfo, retrieveStudy);

					_scus.Add(scu);
					//don't block the calling thread to do this.
					ThreadPool.QueueUserWorkItem(delegate(object theScu) { ((RetrieveScu)theScu).Retrieve(); }, scu);
				}
			}
		}

		#endregion
	}
}
