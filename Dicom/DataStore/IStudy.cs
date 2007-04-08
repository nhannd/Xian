using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IStudy
    {
		ReadOnlyCollection<ISopInstance> GetSopInstances();
		ReadOnlyCollection<ISeries> GetSeries();
        Uid GetStudyInstanceUid();
        void AddSeries(ISeries series);
        void RemoveSeries(ISeries series);
    }
}
