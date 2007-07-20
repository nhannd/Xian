using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDicomPersistentStore
    {
		void InsertSopInstance(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset, string fileName);
        int GetCachedStudiesCount();
        void Flush();
    }
}
