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
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class LocalDataStoreServiceExtension : WcfShred
	{
		private static readonly string _localDataStoreEndpointName = "LocalDataStore";
		private static readonly string _localDataStoreActivityMonitorEndpointName = "LocalDataStoreActivityMonitor";

		private bool _localDataStoreWCFInitialized;
		private bool _localDataStoreActivityMonitorWCFInitialized;

		public LocalDataStoreServiceExtension()
		{
			_localDataStoreWCFInitialized = false;
			_localDataStoreActivityMonitorWCFInitialized = false;
		}

		public override void Start()
		{
			try
			{
				LocalDataStoreService.Instance.Start();
				string message = String.Format(SR.FormatServiceStartedSuccessfully, SR.LocalDataStore);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatServiceFailedToStart, SR.LocalDataStore));
				return;
			}

			try
			{
				StartNetPipeHost<LocalDataStoreServiceType, ILocalDataStoreService>(_localDataStoreEndpointName, SR.LocalDataStore);
				_localDataStoreWCFInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.LocalDataStore);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.LocalDataStore));
			}

			try
			{
				StartNetPipeHost<LocalDataStoreActivityMonitorServiceType, ILocalDataStoreActivityMonitorService>(
					_localDataStoreActivityMonitorEndpointName, SR.LocalDataStoreActivityMonitor);
				_localDataStoreActivityMonitorWCFInitialized = true;
				string message = String.Format(SR.FormatWCFServiceStartedSuccessfully, SR.LocalDataStoreActivityMonitor);
				Platform.Log(LogLevel.Info, message);
				Console.WriteLine(message);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Console.WriteLine(String.Format(SR.FormatWCFServiceFailedToStart, SR.LocalDataStoreActivityMonitor));
			}
		}

		public override void Stop()
		{
			if (_localDataStoreWCFInitialized)
			{
				try
				{
					StopHost(_localDataStoreEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.LocalDataStore));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			if (_localDataStoreActivityMonitorWCFInitialized)
			{
				try
				{
					StopHost(_localDataStoreActivityMonitorEndpointName);
					Platform.Log(LogLevel.Info, String.Format(SR.FormatWCFServiceStoppedSuccessfully, SR.LocalDataStoreActivityMonitor));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}

			try
			{
				LocalDataStoreService.Instance.Stop();
				Platform.Log(LogLevel.Info, String.Format(SR.FormatServiceStoppedSuccessfully, SR.LocalDataStore));
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		public override string GetDisplayName()
		{
			return SR.LocalDataStore;
		}

		public override string GetDescription()
		{
			return SR.LocalDataStoreDescription;
		}
	}
}
