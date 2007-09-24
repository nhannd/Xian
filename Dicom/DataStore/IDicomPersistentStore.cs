using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDicomPersistentStore : IDisposable
    {
		void ClearAllStudies();
    	void RemoveStudies(IEnumerable<IStudy> studies);
		void RemoveStudy(IStudy studies);

    	void InsertSopInstance(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset, string fileName);
    	void Commit();
    }
}
