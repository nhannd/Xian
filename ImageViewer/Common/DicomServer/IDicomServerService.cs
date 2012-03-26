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
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    //TODO (Marmot): Rename to IDicomServer. Get rid of send/retrieve methods.

    /// <summary>
	/// WCF service interface to the Dicom Server.  The Dicom Server runs in a Shred, and the WCF interface allows
	/// external processes, such as the Viewer, to make requests via this interface.
	/// </summary>
	[ServiceContract(ConfigurationName = "IDicomServerService")]
	public interface IDicomServerService
	{
		/// <summary>
		/// Send studies to another Dicom Server.
		/// </summary>
		[Obsolete("Use IDicomSendService instead.")]
		[OperationContract(IsOneWay = true)]
        void Send(ApplicationEntity destinationAEInformation, IEnumerable<string> studyInstanceUids);

		/// <summary>
		/// Performs a study level retrieve from another Dicom Server.  Image level retrieves will not
		/// work using this method and are currently unsupported.
		/// </summary>
		/// <param name="sourceAEInformation">The Dicom server to retrieve from</param>
		/// <param name="studiesToRetrieve">The studies to retrieve.  At an absolute minimum, each <see cref="StudyInformation"/>
		/// object passed in must have the <see cref="StudyInformation.StudyInstanceUid"/> field populated.</param>
		[OperationContract(IsOneWay = true)]
        void RetrieveStudies(ApplicationEntity sourceAEInformation, IEnumerable<StudyInformation> studiesToRetrieve);

		/// <summary>
		/// Performs a series level retrieve from another Dicom Server.  Image level retrieves will not
		/// work using this method and are currently unsupported.
		/// </summary>
		/// <param name="sourceAEInformation">The Dicom server to retrieve from</param>
		/// <param name="seriesToRetrieve">The series to retrieve.  At an absolute minimum, each <see cref="SeriesInformation"/>
		/// object passed in must have the <see cref="SeriesInformation.StudyInstanceUid"/> and <see cref="SeriesInformation.SeriesInstanceUid"/> 
		/// fields populated.</param>
		[OperationContract(IsOneWay = true)]
        void RetrieveSeries(ApplicationEntity sourceAEInformation, StudyInformation studyInformation, IEnumerable<string> seriesInstanceUids);

        /// <summary>
        /// Requests that the DICOM listener be restarted. Would normally be called
        /// after a server configuration change via <see cref="IDicomServerConfiguration.UpdateConfiguration"/>.
        /// </summary>
        [OperationContract]
        RestartListenerResult RestartListener(RestartListenerRequest request);
	}
}
