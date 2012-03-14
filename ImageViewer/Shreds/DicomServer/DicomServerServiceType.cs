#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

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

		public void Send(AEInformation destinationAEInformation, IEnumerable<string> studyInstanceUids)
		{
			try
			{
				SendStudiesRequest request = new SendStudiesRequest();
				request.DestinationAEInformation = destinationAEInformation;
				request.StudyInstanceUids = studyInstanceUids;
				DicomSendManager.Instance.SendStudies(request, null);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
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
				RetrieveStudiesRequest request = new RetrieveStudiesRequest(sourceAEInformation, studiesToRetrieve);
				DicomRetrieveManager.Instance.RetrieveStudies(request);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				//we throw a serializable, non-FaultException-derived exception so that the 
				//client channel *does* get closed.
				string message = SR.ExceptionFailedToInitiateRetrieve;
				message += "\nDetail: " + e.Message;
				throw new DicomServerException(message);
			}
		}

		public void RetrieveSeries(AEInformation sourceAEInformation, StudyInformation studyInformation, IEnumerable<string> seriesInstanceUids)
		{
			try
			{
				RetrieveSeriesRequest request = new RetrieveSeriesRequest(sourceAEInformation, studyInformation, seriesInstanceUids);
				DicomRetrieveManager.Instance.RetrieveSeries(request);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
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
				Platform.Log(LogLevel.Error, e);
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
				Platform.Log(LogLevel.Error, e);
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
