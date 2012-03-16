#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.ServerTree;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    // TODO (Marmot): remove this stuff, but will have to write something to take it's place first.

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
		private static string _offlineAeTitle = null;

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

				Delegate[] delegates = null;

				lock (_syncLock)
				{
					if (_configuration == value)
						return;

					_configuration = value;
					if (_changed != null)
						delegates = _changed.GetInvocationList();
				}

				if (delegates != null)
				{
					foreach (Delegate @delegate in delegates)
						@delegate.DynamicInvoke(null, EventArgs.Empty);
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

		/// <summary>
		/// Gets the current AETitle if the server is running, or the last known AETitle if the server is not running.
		/// </summary>
		public static string OfflineAETitle
		{
			get { return GetOfflineAETitle(true); }
		}

		public static string GetOfflineAETitle(bool wait)
		{
			if (wait)
			{
				try
				{
					Refresh(false);
					_offlineAeTitle = DicomServerConfiguration.AETitle;
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}
			else
			{
				RefreshAsync();
				if (DicomServerConfiguration != null)
					_offlineAeTitle = DicomServerConfiguration.AETitle;
			}

			if (_offlineAeTitle == null)
				_offlineAeTitle = new ServerTree.ServerTree().RootNode.LocalDataStoreNode.OfflineAE;

			return _offlineAeTitle;
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
				throw new RefreshException("Failed to get the DICOM server configuration; the service may not be running.", e);
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
			var client = new DicomServerServiceClient();

			try
			{
			    var configuration = new DicomServerConfiguration
			                            {
			                                HostName = hostName,
			                                AETitle = aeTitle,
			                                Port = port,
			                                InterimStorageDirectory = interimStorageDirectory
			                            };

				client.UpdateServerConfiguration(configuration);
				client.Close();
			}
			catch (Exception e)
			{
				client.Abort();
				throw new UpdateException("Failed to update the DICOM server configuration; the service may not be running.", e);
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
