using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IStudy
    {
        IEnumerable<ISopInstance> GetSopInstances();
        IEnumerable<ISeries> GetSeries();
        Uid GetStudyInstanceUid();
        void AddSeries(ISeries series);
        void RemoveSeries(ISeries series);
    }
}
