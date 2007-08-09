using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using MySR = ClearCanvas.Dicom.SR;

namespace ClearCanvas.Dicom.OffisNetwork
{
	public sealed class DicomServer
    {
		private class AssociationProcessor
		{
			private T_ASC_Association _association;
			private DicomServer _parent;

			public AssociationProcessor(DicomServer parent, T_ASC_Association association)
			{
				_parent = parent;
				_association = association;
			}

			public void Process()
			{
				//this class is responsible for the association's disposal.
				using (_association)
				{
					bool needRelease = true;

					try
					{
						OFCondition cond = _association.ProcessServerCommands(-1, _parent.SaveDirectory);
						if (OffisDicomHelper.CompareConditions(cond, OffisDcm.DUL_PEERREQUESTEDRELEASE))
						{
							needRelease = false;
							_association.RespondToReleaseRequest();
						}
						else
						{
							needRelease = false;
							_association.Release();
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}

					try
					{
						if (needRelease)
							_association.Release();
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}

					_parent.RemoveThread(Thread.CurrentThread);
				}
			}
		}

		private static readonly int _timeout = 500;
		private static readonly int _operationTimeout = 5;
		private static readonly short _maximumAssociations = 25;

		private object _startStopSyncLock = new object();
		private Thread _waitForIncomingAssociationsThread;
		private volatile bool _stopWaitForIncomingAssociationsThread;
		private Exception _startException;

		private readonly ApplicationEntity _myOwnAE;
		private readonly string _saveDirectory;

		private object _syncLock = new object();
		private List<Thread> _threadList;

		public DicomServer(ApplicationEntity ownAEParameters, string saveDirectory)
        {
			_myOwnAE = ownAEParameters;
            _saveDirectory = OffisDicomHelper.NormalizeDirectory(saveDirectory);
			_threadList = new List<Thread>();
        }

		private DicomServer() 
		{
		}

		#region Public Members

		public bool IsRunning
		{
			get
			{ 
				lock(_startStopSyncLock)
				{
					return _waitForIncomingAssociationsThread != null;
				}
			}
		}

		public string AETitle
		{
			get{ return _myOwnAE.AE; }
		}

		public string HostName
		{
			get { return _myOwnAE.Host; }
		}

		public int ListeningPort
		{
			get { return _myOwnAE.Port; }
		}

		public String SaveDirectory
		{
			get { return _saveDirectory; }
		}

		public void Start()
        {
			lock (_startStopSyncLock)
			{
				if (_waitForIncomingAssociationsThread != null)
					return;

				_stopWaitForIncomingAssociationsThread = false;
				_waitForIncomingAssociationsThread = new Thread(new ThreadStart(WaitForIncomingAssociations));
				_waitForIncomingAssociationsThread.IsBackground = true;
				_waitForIncomingAssociationsThread.Priority = ThreadPriority.BelowNormal;
				_waitForIncomingAssociationsThread.Start();

				//wait for the signal that the thread is finished starting up.
				Monitor.Wait(_startStopSyncLock);
				if (_startException != null)
				{
					_waitForIncomingAssociationsThread = null;
					Exception e = _startException;
					_startException = null;
					throw e;
				}
			}
        }

		public void Stop()
		{
			lock (_startStopSyncLock)
			{
				if (_waitForIncomingAssociationsThread == null)
					return;

				_stopWaitForIncomingAssociationsThread = true;
				_waitForIncomingAssociationsThread.Join();
				_waitForIncomingAssociationsThread = null;
			}
		}

		#endregion

		#region Private members

		private void WaitForIncomingAssociations()
        {
			SocketManager.InitializeSockets();
			T_ASC_Network network;

			lock (_startStopSyncLock)
			{
				try
				{
					network = new T_ASC_Network(T_ASC_NetworkRole.NET_ACCEPTOR, _myOwnAE.Port, _timeout);
				}
				catch (Exception e)
				{
					_startException = e;
					SocketManager.DeinitializeSockets();
					return;
				}
				finally
				{
					Monitor.Pulse(_startStopSyncLock);
				}
			}

			using (network)
			{
				do
				{
					try
					{
						T_ASC_Association association = network.AcceptAssociation(_myOwnAE.AE, _operationTimeout, 0, _maximumAssociations);

						if (null != association)
						{
							// spawn a thread to process the association
							AssociationProcessor processor = new AssociationProcessor(this, association);
							Thread thread = new Thread(new ThreadStart(processor.Process));
							thread.Priority = ThreadPriority.BelowNormal;
							thread.IsBackground = true;
							AddThread(thread);
							thread.Start();
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}

				} while (!_stopWaitForIncomingAssociationsThread);

				//a thread could be finishing up and removing itself at the same time as we are iterating,
				//so we take a copy of the thread list to join all the threads.
				List<Thread> snapshot = new List<Thread>();
				lock (_syncLock)
				{
					snapshot.InsertRange(0, _threadList);
				}

				// wait for all the child processing threads to complete
				snapshot.ForEach(delegate(Thread thread) { if (thread.IsAlive) thread.Join(); });
				_threadList.Clear();
			}

			SocketManager.DeinitializeSockets();
		}

		private void AddThread(Thread thread)
		{
			lock (_syncLock)
			{
				_threadList.Add(thread);
			}
		}

		private void RemoveThread(Thread thread)
		{
			lock (_syncLock)
			{
				_threadList.Remove(thread);
			}
		}

        #endregion
    }
}
