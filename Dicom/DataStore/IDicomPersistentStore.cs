using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDicomPersistentStore
    {
        void InsertSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName);
        int GetCachedStudiesCount();
        void Flush();
    }
}
