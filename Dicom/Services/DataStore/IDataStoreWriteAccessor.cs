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
