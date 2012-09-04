#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    /// <summary>
	/// WCF service interface to the local DICOM server.
	/// </summary>
	[ServiceContract(ConfigurationName = "IDicomServer")]
	public interface IDicomServer
	{
        //TODO (Marmot): Add Start/Stop?
        /// <summary>
        /// Gets the current state of the local DICOM listener.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetListenerStateResult GetListenerState(GetListenerStateRequest request);

        /// <summary>
        /// Requests that the DICOM listener be restarted. Would normally be called
        /// after a server configuration change via <see cref="IDicomServerConfiguration.UpdateConfiguration"/>.
        /// </summary>
        [OperationContract]
        RestartListenerResult RestartListener(RestartListenerRequest request);
	}
}
