using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDicomPersistentStore : IDisposable
    {
    	void UpdateSopInstance(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset, string fileName);
    	void Commit();
    }
}
