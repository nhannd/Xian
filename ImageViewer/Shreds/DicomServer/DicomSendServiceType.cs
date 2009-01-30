using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	public class DicomSendServiceType : IDicomSendService
	{
		public DicomSendServiceType()
		{
		}

		#region IDicomSendService Members

		public SendOperationReference SendStudies(SendStudiesRequest request)
		{
			try
			{
				return DicomSendManager.Instance.SendStudies(request, null);
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


		public SendOperationReference SendSeries(SendSeriesRequest request)
		{
			try
			{
				return DicomSendManager.Instance.SendSeries(request, null);
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

		public SendOperationReference SendSopInstances(SendSopInstancesRequest request)
		{
			try
			{
				return DicomSendManager.Instance.SendSopInstances(request, null);
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

		public SendOperationReference SendFiles(SendFilesRequest request)
		{
			try
			{
				return DicomSendManager.Instance.SendFiles(request, null);
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

		#endregion
	}
}
