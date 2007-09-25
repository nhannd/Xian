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

		int GetNumberOfSopInstances(); 
		IEnumerable<ISopInstance> GetSopInstances();
    }
}
