using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
	public interface IDataStoreStudyRemover : IDisposable
	{
		void ClearAllStudies();
		void RemoveStudies(IEnumerable<Uid> studyUids);
		void RemoveStudy(Uid studyUid);
	}
}
