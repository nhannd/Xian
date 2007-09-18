using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDataStoreWriter
    {
		void StoreSopInstances(IEnumerable<ISopInstance> sop);
		void StoreSopInstance(ISopInstance sop);
        void StoreSeries(ISeries series);
		void StoreStudy(IStudy study);

		void RemoveSopInstances(IEnumerable<ISopInstance> sop);
		void RemoveSopInstance(ISopInstance sop);
		void RemoveSeries(ISeries series);
        void RemoveStudy(IStudy study);
    }
}
