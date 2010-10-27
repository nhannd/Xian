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

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDataStoreWriteAccessor
    {
        ISopInstance NewImageSopInstance();
        void StoreSopInstance(ISopInstance sop);
        void StoreSeries(ISeries series);
        void RemoveSeries(ISeries series);
        void RemoveSopInstance(ISopInstance sop);
        void StoreStudy(IStudy study);
        void RemoveStudy(IStudy study);

        void StoreDictionary(DicomDictionaryContainer container);
    }
}
