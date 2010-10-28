#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDataStore
    {
        bool StudyExists(Uid referencedUid);
        bool SeriesExists(Uid referencedUid);
        bool SopInstanceExists(Uid referencedUid);
        ISopInstance GetSopInstance(Uid referencedUid);
        ISeries GetSeries(Uid referenceUid);
        IStudy GetStudy(Uid referenceUid);
        ReadOnlyQueryResultCollection StudyQuery(QueryKey queryKey);
    }
}
