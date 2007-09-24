using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    internal interface IDataStoreWriter : IDisposable
    {
		void StoreSopInstances(IEnumerable<SopInstance> sops);
        void StoreSeries(IEnumerable<Series> series);
		void StoreStudies(IEnumerable<Study> studies);
    	
		void ClearAllStudies();

		void RemoveSopInstances(IEnumerable<ISopInstance> sops);
		void RemoveSeries(IEnumerable<ISeries> series);
		void RemoveStudies(IEnumerable<IStudy> studies);
	}
}
