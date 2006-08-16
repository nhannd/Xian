using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface ISeries
    {
        Uid GetSeriesInstanceUid();
        IEnumerable<ISopInstance> GetSopInstances();
        void AddSopInstance(ISopInstance sop);
        void RemoveSopInstance(ISopInstance sop);
        void SetParentStudy(IStudy study);
        IStudy GetParentStudy();
    }
}
