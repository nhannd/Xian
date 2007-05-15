using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;
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
    public class DiskspaceManagerServiceType : IDiskspaceManagerService
    {
        public DiskspaceManagerServiceType()
        {
		}

		#region IDiskspaceManagerService Members

		public ServiceInformation GetServiceInformation()
		{
			try
			{
				return DiskspaceManagerProcessor.Instance.GetServiceInformation();
			}
			catch (Exception e)
			{
				Platform.Log(e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToGetServerConfiguration;
				message += "\nDetail: " + e.Message;
				throw new DiskspaceManagerException(message);
			}
		}

		public void UpdateServiceConfiguration(ServiceConfiguration newConfiguration)
		{
			try
			{
				DiskspaceManagerProcessor.Instance.UpdateServiceConfiguration(newConfiguration);
			}
			catch (Exception e)
			{
				Platform.Log(e);
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
