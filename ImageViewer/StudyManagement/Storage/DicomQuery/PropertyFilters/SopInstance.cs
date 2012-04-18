using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class SopInstanceUniqueKey
    {
        internal class StudyInstanceUid : UidPropertyFilter<SopInstance>
        {
            public StudyInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.StudyInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.StudyInstanceUid));
            }

            protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.StudyInstanceUid);
            }
        }

        internal class SeriesInstanceUid : UidPropertyFilter<SopInstance>
        {
            public SeriesInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.SeriesInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.SeriesInstanceUid));
            }

            protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.SeriesInstanceUid);
            }
        }

        internal class SopInstanceUid : UidPropertyFilter<SopInstance>
        {
            public SopInstanceUid(DicomAttributeCollection criteria)
                : base(DicomTags.SopInstanceUid, criteria)
            {
                IsReturnValueRequired = true;
                AddToQueryEnabled = false;
                FilterResultsEnabled = true;
            }

            protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
            {
                var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
                return results.Where(s => criterion.Contains(s.SopInstanceUid));
            }

            protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
            {
                resultAttribute.SetStringValue(item.SopInstanceUid);
            }
        }
    }

    internal class InstanceNumber : DicomPropertyFilter<SopInstance>
    {
        public InstanceNumber(DicomAttributeCollection criteria)
            : base(DicomTags.InstanceNumber, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = true;
        }

        protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
        {
            var criterion = Criterion.GetInt32(0, 0);
            return results.Where(s => s.InstanceNumber == criterion);
        }

        protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetInt32(0, item.InstanceNumber);
        }
    }

    internal class SopClassUid : UidPropertyFilter<SopInstance>
    {
        public SopClassUid(DicomAttributeCollection criteria)
            : base(DicomTags.SopClassUid, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = true;
        }

        protected override IEnumerable<SopInstance> FilterResults(IEnumerable<SopInstance> results)
        {
            var criterion = DicomStringHelper.GetStringArray(Criterion.ToString());
            return results.Where(s => criterion.Contains(s.SopClassUid));
        }

        protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetStringValue(item.SopClassUid);
        }
    }
}
