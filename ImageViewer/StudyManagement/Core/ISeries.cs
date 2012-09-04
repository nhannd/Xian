#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    public interface ISeries : ISeriesData
    {
        IStudy GetParentStudy();
        IEnumerable<ISopInstance> GetSopInstances();

        string SpecificCharacterSet { get; }
    }
}