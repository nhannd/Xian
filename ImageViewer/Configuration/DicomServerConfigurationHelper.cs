using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.Services.DicomServer;
using System.Threading;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Configuration
{
	public static class DicomServerConfigurationHelper
	{
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
				DicomServerConfigurationHelper.Refresh();
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

		private static object _syncLock = new object();
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
				Refresh();
				return DicomServerConfiguration.HostName; 
			}
		}

		public static string AETitle
		{
			get
			{
				Refresh();
				return DicomServerConfiguration.AETitle; 
			}
		}

		public static int Port
		{
			get
			{
				Refresh();
				return DicomServerConfiguration.Port; 
			}
		}

		public static string InterimStorageDirectory
		{
			get 
			{
				Refresh();
				return DicomServerConfiguration.InterimStorageDirectory; 
			}
		}

		public static IDicomServerConfigurationProvider GetDicomServerConfigurationProvider()
		{
			return new DicomServerConfigurationProvider();
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

		internal static void Refresh()
		{
			Refresh(false);
		}

		internal static void Refresh(bool force)
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
				throw new Exception(SR.ExceptionDicomServerConfigurationRefreshFailed, e);
			}
			finally
			{
				lock (_syncLock)
				{
					_refreshing = false;
				}
			}
		}

		internal static void Update(string hostName, string aeTitle, int port, string interimStorageDirectory)
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
				throw new Exception(SR.ExceptionDicomServerConfigurationUpdateFailed, e);
			}
			finally
			{ 
				RefreshAsync(true);
			}
		}
	}
}
