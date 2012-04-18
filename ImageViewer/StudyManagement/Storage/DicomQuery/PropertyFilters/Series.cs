using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class SeriesUniqueKey
    {
        internal class StudyInstanceUid : UidPropertyFilter<Series, SeriesEntry>
        {
            public StudyInstanceUid(DicomAttributeCollection criteria) 
                : base(DicomTags.StudyInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<Series> FilterResults(IEnumerable<Series> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.StudyInstanceUid));
            }

            protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.StudyInstanceUid);
            }
        }

        internal class SeriesInstanceUid : UidPropertyFilter<Series, SeriesEntry>
        {
            public SeriesInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.SeriesInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<Series> FilterResults(IEnumerable<Series> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.SeriesInstanceUid));
            }

            protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.SeriesInstanceUid);
            }
        }
    }

    internal class Modality : StringPropertyFilter<Series, SeriesEntry>
    {
        public Modality(DicomAttributeCollection criteria)
            : base(DicomTags.Modality, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = true;
        }

        protected override string GetPropertyValue(Series item)
        {
            return item. Modality;
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.Modality);
        }
    }

    internal class SeriesDescription : StringPropertyFilter<Series, SeriesEntry>
    {
        public SeriesDescription(DicomAttributeCollection criteria)
            : base(DicomTags.SeriesDescription, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = true;
        }

        protected override string GetPropertyValue(Series item)
        {
            return item.SeriesDescription;
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SeriesDescription);
        }
    }

    internal class SeriesNumber : DicomPropertyFilter<Series, SeriesEntry>
    {
        public SeriesNumber(DicomAttributeCollection criteria)
            : base(DicomTags.SeriesNumber, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = true;
        }

        protected override IEnumerable<Series> FilterResults(IEnumerable<Series> results)
        {
            var criterion = Criterion.GetInt32(0, 0);
            return results.Where(s => s.SeriesNumber == criterion);
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetInt32(0, item.SeriesNumber);
        }
    }

    internal class NumberOfSeriesRelatedInstances : DicomPropertyFilter<Series, SeriesEntry>
    {
        public NumberOfSeriesRelatedInstances(DicomAttributeCollection criteria)
            : base(DicomTags.NumberOfSeriesRelatedInstances, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = true;
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetInt32(0, item.NumberOfSeriesRelatedInstances);
        }
    }
}
