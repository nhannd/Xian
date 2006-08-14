using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Utilities.RebuildDatabase
{
    public interface IDicomPersistentStore
    {
        void InsertSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName);
        int GetCachedStudiesCount();
        void Flush();
    }
}
