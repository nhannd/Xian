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

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    // Note (Marmot): Leave this for now because it's not that bad, and it's tied into
    // the "My Studies" node in the Server Tree. Could change it later.
    public interface IDicomServerConfigurationProvider
    {
        string AETitle { get; }
        [Obsolete("Use HostName instead.")]
        string Host { get; }
        string HostName { get; }
        int Port { get; }
        string FileStoreDirectory { get; }

        bool NeedsRefresh { get; }
        bool ConfigurationExists { get; }

        void Refresh();
        void RefreshAsync();

        event EventHandler Changed;
    }

    //NOTE (Marmot): Leave this stuff for now, as it serves a purpose for the "My Studies" node
    //of the server tree and also, it's an easy way to get the local AE title for auditing, etc.
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

			public string AETitle
			{
				get { return DicomServerConfigurationHelper.AETitle; }
			}

            public string Host
            {
                get { return HostName; }
            }

            public string HostName
            {
                get { return DicomServerConfigurationHelper.HostName; }
            }

			public int Port
			{
				get { return DicomServerConfigurationHelper.Port; }
			}

            public string FileStoreDirectory
			{
				get { return DicomServerConfigurationHelper.FileStoreDirectory; }
			}

			public bool ConfigurationExists
			{
				get { return DicomServerConfiguration != null; }
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

				Delegate[] delegates = null;

				lock (_syncLock)
				{
					if (Equals(value, _configuration))
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
		
        [Obsolete("Just use the AETitle property instead.")]
        public static string OfflineAETitle
        {
            get { return AETitle; }
        }
        
        public static string AETitle
		{
			get
			{
			    //TODO (Marmot): Make sure usages of this are handling the (unlikely) possibility of an exception being thrown.
				Refresh(false);
				return DicomServerConfiguration.AETitle; 
			}
		}
        
        [Obsolete("Use HostName instead.")]
	    public static string Host
	    {
            get { return HostName; }
	    }

        public static string HostName
        {
            get
            {
                Refresh(false);
                return DicomServerConfiguration.HostName;
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

        public static string FileStoreDirectory
		{
			get 
			{
				Refresh(false);
				return DicomServerConfiguration.FileStoreDirectory; 
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
				
			WaitCallback del = delegate
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

			try
			{
			    var request = new GetDicomServerConfigurationRequest();
                Platform.GetService<IDicomServerConfiguration>(s => 
                            DicomServerConfiguration = s.GetConfiguration(request).Configuration);
			}
			catch (Exception e)
			{
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
			try
			{
			    Platform.GetService<IDicomServerConfiguration>(s =>
                                   {
                                       var configuration = new DicomServerConfiguration
                                                               {
                                                                   HostName = hostName,
                                                                   AETitle = aeTitle,
                                                                   Port = port,
                                                                   FileStoreDirectory = interimStorageDirectory
                                                               };

                                       var request = new UpdateDicomServerConfigurationRequest {Configuration = configuration};
                                       s.UpdateConfiguration(request);
                                   });
			}
			catch (Exception e)
			{
				throw new UpdateException("Failed to update the DICOM server configuration; the service may not be running.", e);
			}
			finally
			{ 
				RefreshAsync(true);
			}
		}

		public static IDicomServerConfigurationProvider GetConfigurationProvider()
		{
			return new DicomServerConfigurationProvider();
		}

        [Obsolete("Just use the AETitle property instead.")]
        public static string GetOfflineAETitle(bool wait)
        {
            return AETitle;
        }
    }
}
