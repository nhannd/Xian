using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    //TODO (Marmot): Change to use service nodes so we don't need to use the term "local".
    public abstract class StudyStore : IStudyStore
    {
        static StudyStore()
        {
            try
            {
                var service = Platform.GetService<IStudyStore>();
                IsSupported = service != null;
                var disposable = service as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            catch (NotSupportedException)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, "Study Store is not supported.");
            }
            catch (Exception e)
            {
                IsSupported = false;
                Platform.Log(LogLevel.Debug, e, "Study Store is not supported.");
            }
        }

        public static bool IsSupported { get; private set; }

        public abstract GetStudyCountResult GetStudyCount(GetStudyCountRequest request);

        public abstract IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria);

        public abstract IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria);

        public abstract IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria);
    }
}
