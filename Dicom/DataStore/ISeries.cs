using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface ISeries
    {
        Uid GetSeriesInstanceUid();

		IStudy GetParentStudy();

    	int GetNumberOfSopInstances();
		IEnumerable<ISopInstance> GetSopInstances();
    }
}
