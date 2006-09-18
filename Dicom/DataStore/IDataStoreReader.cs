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
        ISopInstance GetSopInstance(Uid referencedUid);
        ISeries GetSeries(Uid referenceUid);
        ISeries GetSeriesAndSopInstances(Uid referenceUid);
        IStudy GetStudy(Uid referenceUid);
        IStudy GetStudyAndSeries(Uid referenceUid);
        IStudy GetStudyAndAllObjects(Uid referenceUid);
        void InitializeAssociatedObject(object primaryObject, object associatedObject);
        ReadOnlyQueryResultCollection StudyQuery(QueryKey queryKey);
    }
}
