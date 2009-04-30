#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;
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
				                         configuration.Port, configuration.InterimStorageDirectory);
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

				return new DicomServerConfiguration(DicomServerSettings.Instance.HostName,
												DicomServerSettings.Instance.AETitle,
												DicomServerSettings.Instance.Port,
												DicomServerSettings.Instance.InterimStorageDirectory);
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
				DicomServerSettings.Instance.InterimStorageDirectory = newConfiguration.InterimStorageDirectory;
				DicomServerSettings.Save();

				OnConfigurationChanged();
			}
		}

		#endregion
	}
}
