using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    public class LocalStudyRootQuery : IStudyRootQuery, IDisposable
    {
        static LocalStudyRootQuery()
        {
            IsSupported = new LocalStudyRootQueryExtensionPoint().ListExtensions().Length > 0;
        }

        private IStudyRootQuery _real;

        public LocalStudyRootQuery()
        {
            _real = new LocalStudyRootQueryExtensionPoint().CreateExtension() as IStudyRootQuery;
        }

        public static bool IsSupported { get; private set; }

        public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            return _real.StudyQuery(queryCriteria);
        }

        public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            return _real.SeriesQuery(queryCriteria);
        }

        public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            return _real.ImageQuery(queryCriteria);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_real == null)
                return;

            try
            {
                if (_real is IDisposable)
                    ((IDisposable) _real).Dispose();
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Debug, e, "Error disposing local study root query.");
            }

            _real = null;
        }

        #endregion

        public static IStudyRootQuery Create()
        {
            return IsSupported ? new LocalStudyRootQuery() : null;
        }
    }
}
