#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class DicomServerManager
	{
		private enum ServerState
		{
			Stopped = 0,
			Starting,
			Started,
			Stopping
		}

		public static readonly DicomServerManager Instance = new DicomServerManager();

		#region Private Fields

		private readonly object _syncLock = new object();
		private DicomServer _server;
		private ServerState _serverState;

		private bool _active;
		private bool _restart;

		#endregion

		private DicomServerManager()
		{
		}

		#region Private Methods

		private void StartServerAsync(object nothing)
		{
			DicomServerConfiguration configuration;
			lock (_syncLock)
			{
				configuration = GetServerConfiguration();
				_restart = false;
			}

			DicomServer server = null;

			try
			{
				Trace.WriteLine("Starting Dicom server.");

				server = new DicomServer(configuration.AETitle, configuration.HostName,
				                         configuration.Port, configuration.FileStoreLocation);
				server.Start();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Failed to start dicom server ({0}/{1}:{2}",
				             configuration.HostName, configuration.AETitle, configuration.Port);

				server = null;
			}
			finally
			{
				lock (_syncLock)
				{
					//the server may be null here, we are just reflecting the state based on the method calls.
					_server = server;
					_serverState = ServerState.Started;
					OnServerStarted();
				}
			}
		}

		private void StopServerAsync(object nothing)
		{
			lock (_syncLock)
			{
				DicomServer server = _server;
				_server = null;

				if (server != null)
				{
					Trace.WriteLine("Stopping Dicom server.");
					server.Stop();
				}

				_serverState = ServerState.Stopped;
				OnServerStopped();
			}
		}

		private void OnServerStarted()
		{
			lock (_syncLock)
			{
				Monitor.Pulse(_syncLock);
				if (!_active)
					_restart = false;

				Trace.WriteLine("Started Dicom server.");

				if (_restart)
					StopServer(false);
			}
		}

		private void OnServerStopped()
		{
			lock (_syncLock)
			{
				Monitor.Pulse(_syncLock);
				if (!_active)
					_restart = false;

				Trace.WriteLine("Stopped Dicom server.");

				if (_restart)
					StartServer(false);
			}
		}

		private void StartServer(bool wait)
		{
			lock (_syncLock)
			{
				if (_serverState == ServerState.Stopped)
				{
					_serverState = ServerState.Starting;
					ThreadPool.QueueUserWorkItem(StartServerAsync);
				}

				if (wait)
				{
					while (_serverState != ServerState.Started)
						Monitor.Wait(_syncLock, 50);
				}
			}
		}

		private void StopServer(bool wait)
		{
			lock (_syncLock)
			{
				if (_serverState == ServerState.Started)
				{
					_serverState = ServerState.Stopping;
					ThreadPool.QueueUserWorkItem(StopServerAsync);
				}

				if (wait)
				{
					while (_serverState != ServerState.Stopped)
						Monitor.Wait(_syncLock, 50);
				}
			}
		}

		private void OnConfigurationChanged()
		{
			lock (_syncLock)
			{
				if (_active)
				{
					Trace.WriteLine("Configuration change detected - restarting Dicom server.");
					_restart = true;
					StopServer(false);
				}
			}
		}

		#endregion

		#region Public Methods

		public void Start()
		{
			lock (_syncLock)
			{
				_active = true;
				StartServer(true);
			}
		}

		public void Stop()
		{
			lock (_syncLock)
			{
				_active = false;
				_restart = false;
				StopServer(true);
			}
		}

		public DicomServerConfiguration GetServerConfiguration()
		{
			lock (_syncLock)
			{
				if (!_active)
					throw new InvalidOperationException("The Dicom Server service is not active.");

                return new DicomServerConfiguration
                    {
                        HostName = DicomServerSettings.Instance.HostName,
                        AETitle = DicomServerSettings.Instance.AETitle,
                        Port = DicomServerSettings.Instance.Port,
                        FileStoreLocation = DicomServerSettings.Instance.InterimStorageDirectory
                    };
			}
		}

		public void UpdateServerConfiguration(DicomServerConfiguration newConfiguration)
		{
			lock (_syncLock)
			{
				if (!_active)
					throw new InvalidOperationException("The Dicom Server service is not active.");

				DicomServerSettings.Instance.HostName = newConfiguration.HostName;
				DicomServerSettings.Instance.AETitle = newConfiguration.AETitle;
				DicomServerSettings.Instance.Port = newConfiguration.Port;
				DicomServerSettings.Instance.InterimStorageDirectory = newConfiguration.FileStoreLocation;
				DicomServerSettings.Save();

				OnConfigurationChanged();
			}
		}

		#endregion
	}
}
