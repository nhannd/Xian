using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDataStoreReader
    {
		bool StudyExists(Uid referencedUid);
        bool SeriesExists(Uid referencedUid);
        bool SopInstanceExists(Uid referencedUid);

		IStudy GetStudy(Uid referenceUid);
		ISeries GetSeries(Uid referenceUid);
		ISopInstance GetSopInstance(Uid referencedUid);

        IEnumerable<IStudy> GetStudies();
		ReadOnlyQueryResultCollection StudyQuery(QueryKey queryKey);
    }
}
