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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.Services.DicomServer;
using System.Threading;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.DicomServer
{
	public static class DicomServerConfigurationHelper
	{
		[Serializable]
		public class UpdateException : Exception
		{
			internal UpdateException(string message)
				: base(message)
			{
			}

			internal UpdateException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
		}

		[Serializable]
		public class RefreshException : Exception
		{
			internal RefreshException(string message)
				: base(message)
			{
			}

			internal RefreshException(string message, Exception innerException)
				: base(message, innerException)
			{
			}
		}

		private class DicomServerConfigurationProvider : IDicomServerConfigurationProvider
		{
			internal DicomServerConfigurationProvider()
			{
			}

			#region IDicomServerConfigurationProvider Members

			public string Host
			{
				get { return DicomServerConfigurationHelper.Host; }
			}

			public string AETitle
			{
				get { return DicomServerConfigurationHelper.AETitle; }
			}

			public int Port
			{
				get { return DicomServerConfigurationHelper.Port; }
			}

			public string InterimStorageDirectory
			{
				get { return DicomServerConfigurationHelper.InterimStorageDirectory; }
			}

			public bool ConfigurationExists
			{
				get { return DicomServerConfigurationHelper.DicomServerConfiguration != null; }
			}

			public bool NeedsRefresh
			{
				get { return DicomServerConfigurationHelper.NeedsRefresh; }
			}

			public void Refresh()
			{
				DicomServerConfigurationHelper.Refresh(false);
			}

			public void RefreshAsync()
			{
				DicomServerConfigurationHelper.RefreshAsync();
			}

			public event EventHandler Changed
			{
				add { DicomServerConfigurationHelper.Changed += value; }
				remove { DicomServerConfigurationHelper.Changed -= value; }
			}

			#endregion
		}

		private static readonly object _syncLock = new object();
		private static DicomServerConfiguration _configuration;
		private static event EventHandler _changed;
		private static bool _refreshing;

		internal static event EventHandler Changed
		{
			add 
			{
				lock (_syncLock)
				{
					_changed += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_changed -= value;
				}
			}
		}

		internal static bool NeedsRefresh
		{
			get
			{
				lock (_syncLock)
				{
					return !_refreshing && _configuration == null;
				}
			}
		}

		internal static bool Refreshing
		{
			get
			{
				lock (_syncLock)
				{
					return _refreshing;
				}
			}
		}

		internal static DicomServerConfiguration DicomServerConfiguration
		{
			get
			{
				lock (_syncLock)
				{
					return _configuration;
				}
			}
			set
			{
				lock (_syncLock)
				{
					if (_configuration == value)
						return;

					_configuration = value;
					EventsHelper.Fire(_changed, null, EventArgs.Empty);
				}
			}
		}
		
		public static string Host
		{
			get
			{
				Refresh(false);
				return DicomServerConfiguration.HostName; 
			}
		}

		public static string AETitle
		{
			get
			{
				Refresh(false);
				return DicomServerConfiguration.AETitle; 
			}
		}

		public static int Port
		{
			get
			{
				Refresh(false);
				return DicomServerConfiguration.Port; 
			}
		}

		public static string InterimStorageDirectory
		{
			get 
			{
				Refresh(false);
				return DicomServerConfiguration.InterimStorageDirectory; 
			}
		}

		internal static void RefreshAsync()
		{
			RefreshAsync(false);
		}

		internal static void RefreshAsync(bool force)
		{
			if (Refreshing)
				return;
				
			WaitCallback del = delegate(object nothing)
			{
				try
				{
					Refresh(force);
				}
				catch (Exception e)
				{
					if (!(e.InnerException is EndpointNotFoundException))
						Platform.Log(LogLevel.Error, e);
				}
			};

			ThreadPool.QueueUserWorkItem(del);
		}

		public static void Refresh(bool force)
		{
			lock (_syncLock)
			{
				if (_refreshing || (_configuration != null && !force))
					return;

				_refreshing = true;
			}

			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				DicomServerConfiguration = client.GetServerConfiguration();
				client.Close();
			}
			catch (Exception e)
			{
				client.Abort();
				throw new RefreshException("Failed to get the Dicom server configuration; the service may not be running.", e);
			}
			finally
			{
				lock (_syncLock)
				{
					_refreshing = false;
				}
			}
		}

		public static void Update(string hostName, string aeTitle, int port, string interimStorageDirectory)
		{
			DicomServerServiceClient client = new DicomServerServiceClient();

			try
			{
				DicomServerConfiguration configuration = new DicomServerConfiguration(hostName, aeTitle, port, interimStorageDirectory);

				client.UpdateServerConfiguration(configuration);
				client.Close();
			}
			catch (Exception e)
			{
				client.Abort();
				throw new UpdateException("Failed to update the Dicom server configuration; the service may not be running.", e);
			}
			finally
			{ 
				RefreshAsync(true);
			}
		}

		internal static IDicomServerConfigurationProvider GetDicomServerConfigurationProvider()
		{
			return new DicomServerConfigurationProvider();
		}
	}
}
