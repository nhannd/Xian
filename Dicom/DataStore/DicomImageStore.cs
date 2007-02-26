using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.DataStore
{
    /// <summary>
    /// Assumes that the objects have non-null and valid UIDs.
    /// </summary>
    public class DicomImageStore : DicomImageStoreBase, IDicomPersistentStore
    {
        #region IDicomPersistentStore Members

        public void InsertSopInstance(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset, string fileName)
        {
            ISopInstance newSop = GetSopInstance(metaInfo, sopInstanceDataset, fileName);
            IStudy study = GetStudy(metaInfo, sopInstanceDataset);
            ISeries series = GetSeries(metaInfo, sopInstanceDataset);

            series.AddSopInstance(newSop);
            study.AddSeries(series);
        }

        public void Flush()
        {
            foreach (KeyValuePair<string, Study> pair in this.StudyCache)
            {
                DataAccessLayer.GetIDataStoreWriter().StoreStudy(pair.Value);
            }
            
            this.SeriesCache.Clear();
            this.StudyCache.Clear();
        }

        public int GetCachedStudiesCount()
        {
            return this.StudyCache.Count;
        }

        #endregion

        #region Handcoded Members
        private IStudy GetStudy(DcmMetaInfo metaInfo, DcmDataset sopInstanceDataset)
        {
            return base.GetStudy(metaInfo, sopInstanceDataset, typeof(DataAccessLayer));
        }
        #endregion
    }
}
