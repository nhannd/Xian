#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common;
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
    public class DicomServerServiceType : IDicomServer
    {
    	#region IDicomServer Members

        public RestartListenerResult RestartListener(RestartListenerRequest request)
        {
            try
            {
                DicomServerManager.Instance.Restart();
                return new RestartListenerResult();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                //we throw a serializable, non-FaultException-derived exception so that the 
                //client channel *does* get closed.
                string message = SR.ExceptionErrorRestartingDICOMServer;
                message += "\nDetail: " + e.Message;
                throw new DicomServerException(message);
            }
        }

        #endregion
    }
}
