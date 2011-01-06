#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    /// <summary>
    /// Fault contract indicating the requested study cannot be retrieved because it is nearline.
    /// </summary>
    [DataContract]
    public class StudyIsNearlineFault
    {

    }
}