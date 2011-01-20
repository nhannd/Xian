#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.ServiceModel;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    /// <summary>
    /// Defines the interface of a service that provides study header information.
    /// </summary>
    [ServiceContract]
    public interface IHeaderStreamingService
    {
        /// <summary>
        /// Retrieves a stream containing the study header information.
        /// </summary>
        /// <param name="callingAETitle">The AE of the caller</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>The stream containing the study header information in compressed XML format</returns>
        /// <seealso cref="HeaderStreamingParameters"></seealso>
        [OperationContract]
        [FaultContract(typeof(StudyIsInUseFault))]
        [FaultContract(typeof(StudyIsNearlineFault))]
        [FaultContract(typeof(StudyNotFoundFault))]
        [FaultContract(typeof(string))]
        Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters);
    }
}
