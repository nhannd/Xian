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

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DiskspaceManager;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
	[Serializable]
	internal class DiskspaceManagerException : Exception
	{
		public DiskspaceManagerException(string message)
			: base(message)
		{
		}

		protected DiskspaceManagerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public sealed class DiskspaceManagerServiceType : IDiskspaceManagerService
    {
        public DiskspaceManagerServiceType()
        {
		}

		#region IDiskspaceManagerService Members

		public DiskspaceManagerServiceInformation GetServiceInformation()
		{
			try
			{
				return DiskspaceManagerProcessor.Instance.GetServiceInformation();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToGetServerConfiguration;
				message += "\nDetail: " + e.Message;
				throw new DiskspaceManagerException(message);
			}
		}

		public void UpdateServiceConfiguration(DiskspaceManagerServiceConfiguration newConfiguration)
		{
			try
			{
				DiskspaceManagerProcessor.Instance.UpdateServiceConfiguration(newConfiguration);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToUpdateServerConfiguration;
				message += "\nDetail: " + e.Message;
				throw new DiskspaceManagerException(message);
			}
		}

		#endregion
	}
}
