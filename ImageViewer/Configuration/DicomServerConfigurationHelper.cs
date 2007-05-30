using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.Services.DicomServer;

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

			public bool NeedsRefresh
			{
				get { return DicomServerConfigurationHelper.DicomServerConfiguration == null; }
			}

			public void Refresh()
			{
				DicomServerConfigurationHelper.Refresh();
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

		internal static void Refresh()
		{
			Refresh(false);
		}

		internal static void Refresh(bool force)
		{
			if (DicomServerConfiguration != null && !force)
				return;

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
		}

		internal static void Update(string hostName, string aeTitle, int port, string interimStorageDirectory)
		{
			DicomServerServiceClient client = new DicomServerServiceClient();
			
			try
			{
				DicomServerConfiguration configuration = new DicomServerConfiguration(hostName, aeTitle, port, interimStorageDirectory);
				
				client.UpdateServerConfiguration(configuration);
				client.Close();

				DicomServerConfiguration = configuration;
			}
			catch (Exception e)
			{
				//we need a refresh now because we can't know if the values changed or not.
				DicomServerConfiguration = null;
				client.Abort();
				throw new Exception(SR.ExceptionDicomServerConfigurationUpdateFailed, e);
			}
		}
	}
}
