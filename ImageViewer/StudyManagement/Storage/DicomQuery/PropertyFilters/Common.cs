using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    #region InstanceAvailability

    internal class InstanceAvailability<T> : PropertyFilter<T>
    {
        protected InstanceAvailability(IDicomAttributeProvider criteria)
            : base(DicomTags.InstanceAvailability, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = false;
            base.IsReturnValueRequired = true;
        }

        protected override void AddValueToResult(T item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, "ONLINE");
        }
    }

    internal class StudyInstanceAvailability : InstanceAvailability<Study>
    {
        protected StudyInstanceAvailability(IDicomAttributeProvider criteria) : base(criteria)
        {
        }
    }

    internal class SeriesInstanceAvailability : InstanceAvailability<Series>
    {
        protected SeriesInstanceAvailability(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceAvailability : InstanceAvailability<SopInstance>
    {
        protected SopInstanceAvailability(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region RetrieveAETitle

    internal class RetrieveAETitle<T> : PropertyFilter<T>
    {
        protected RetrieveAETitle(IDicomAttributeProvider criteria)
            : base(DicomTags.RetrieveAeTitle, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = false;
            base.IsReturnValueRequired = true;
        }

        protected override void AddValueToResult(T item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, DicomServerConfigurationHelper.AETitle);
        }
    }

    internal class StudyRetrieveAETitle: RetrieveAETitle<Study>
    {
        protected StudyRetrieveAETitle(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    internal class SeriesRetrieveAETitle : RetrieveAETitle<Series>
    {
        protected SeriesRetrieveAETitle(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceRetrieveAETitle : RetrieveAETitle<SopInstance>
    {
        protected SopInstanceRetrieveAETitle(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region Specific Character Set

    internal class SpecificCharacterSet<T> : PropertyFilter<T>
    {
        protected SpecificCharacterSet(IDicomAttributeProvider criteria)
            : base(DicomTags.SpecificCharacterSet, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = false;
            base.IsReturnValueRequired = true;
        }
    }

    internal class StudySpecificCharacterSet : SpecificCharacterSet<Study>
    {
        protected StudySpecificCharacterSet(IDicomAttributeProvider criteria) : base(criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, item.SpecificCharacterSet);
        }
    }

    internal class SeriesSpecificCharacterSet : SpecificCharacterSet<Series>
    {
        protected SeriesSpecificCharacterSet(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, item.SpecificCharacterSet);
        }
    }

    internal class SopInstanceSpecificCharacterSet : SpecificCharacterSet<SopInstance>
    {
        protected SopInstanceSpecificCharacterSet(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(SopInstance item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, item.SpecificCharacterSet);
        }
    }

    #endregion
}