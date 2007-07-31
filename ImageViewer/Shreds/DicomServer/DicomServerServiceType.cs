using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[Serializable]
	internal class DicomServerException : Exception
	{
		public DicomServerException(string message)
			: base(message)
		{ 
		}

		protected DicomServerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerCall)]
    public class DicomServerServiceType : IDicomServerService
    {
        public DicomServerServiceType()
        {
		}

		#region IDicomServerService Members

		public void Send(AEInformation destinationAEInformation, IEnumerable<string> uids)
		{
			try
			{
				DicomServerManager.Instance.Send(destinationAEInformation, uids);
			}
			catch (Exception e)
			{
				Platform.Log(e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToInitiateSend;
				message += "\nDetail: " + e.Message;
				throw new DicomServerException(message);
			}
		}

		public void RetrieveStudies(AEInformation sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve)
		{
			try
			{
				DicomServerManager.Instance.RetrieveStudies(sourceAEInformation, studiesToRetrieve);
			}
			catch (Exception e)
			{
				Platform.Log(e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToInitiateRetrieve;
				message += "\nDetail: " + e.Message;
				throw new DicomServerException(message);
			} 
		}

		public DicomServerConfiguration GetServerConfiguration()
		{
			try
			{
				return DicomServerManager.Instance.GetServerConfiguration();
			}
			catch (Exception e)
			{
				Platform.Log(e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToGetServerConfiguration;
				message += "\nDetail: " + e.Message;
				throw new DicomServerException(message);
			}
		}

		public void UpdateServerConfiguration(DicomServerConfiguration newConfiguration)
		{
			try
			{
				DicomServerManager.Instance.UpdateServerConfiguration(newConfiguration);
			}
			catch (Exception e)
			{
				Platform.Log(e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToUpdateServerConfiguration;
				message += "\nDetail: " + e.Message;
				throw new DicomServerException(message);
			}
		}

		#endregion
	}
}
