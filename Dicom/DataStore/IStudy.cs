using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IStudy
    {
		Uid GetStudyInstanceUid();

		int GetNumberOfSeries();
		IEnumerable<ISeries> GetSeries();

		void AddSeries(ISeries series);
		void RemoveSeries(ISeries series);

		int GetNumberOfSopInstances(); 
		IEnumerable<ISopInstance> GetSopInstances();
    }
}
