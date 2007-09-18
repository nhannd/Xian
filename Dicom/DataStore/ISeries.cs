using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface ISeries
    {
        Uid GetSeriesInstanceUid();

		void SetParentStudy(IStudy study);
		IStudy GetParentStudy();

    	int GetNumberOfSopInstances();
		IEnumerable<ISopInstance> GetSopInstances();
        void AddSopInstance(ISopInstance sop);
        void RemoveSopInstance(ISopInstance sop);
    }
}
