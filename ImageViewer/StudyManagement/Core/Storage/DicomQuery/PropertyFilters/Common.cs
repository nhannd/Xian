using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.PropertyFilters
{
    #region InstanceAvailability

    internal class InstanceAvailability<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        public InstanceAvailability(DicomAttributeCollection criteria)
            : base(DicomTags.InstanceAvailability, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = false;
            IsReturnValueRequired = true;
        }

        protected override void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, "ONLINE");
        }
    }

    internal class StudyInstanceAvailability : InstanceAvailability<Study>
    {
        public StudyInstanceAvailability(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SeriesInstanceAvailability : InstanceAvailability<Series>
    {
        public SeriesInstanceAvailability(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceAvailability : InstanceAvailability<SopInstance>
    {
        public SopInstanceAvailability(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region RetrieveAETitle

    internal class RetrieveAETitle<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        public RetrieveAETitle(DicomAttributeCollection criteria)
            : base(DicomTags.RetrieveAeTitle, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = false;
            IsReturnValueRequired = true;
        }

        protected override void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, ServerDirectory.GetLocalServer().AETitle);
        }
    }

    internal class StudyRetrieveAETitle: RetrieveAETitle<Study>
    {
        public StudyRetrieveAETitle(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SeriesRetrieveAETitle : RetrieveAETitle<Series>
    {
        public SeriesRetrieveAETitle(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceRetrieveAETitle : RetrieveAETitle<SopInstance>
    {
        public SopInstanceRetrieveAETitle(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region Specific Character Set

    internal abstract class SpecificCharacterSet<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        protected SpecificCharacterSet(DicomAttributeCollection criteria)
            : base(DicomTags.SpecificCharacterSet, criteria)
        {
            AddToQueryEnabled = false;
            FilterResultsEnabled = false;
            IsReturnValueRequired = true;
        }
    }

    internal class StudySpecificCharacterSet : SpecificCharacterSet<Study>
    {
        public StudySpecificCharacterSet(DicomAttributeCollection criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, item.SpecificCharacterSet);
        }
    }

    internal class SeriesSpecificCharacterSet : SpecificCharacterSet<Series>
    {
        public SeriesSpecificCharacterSet(DicomAttributeCollection criteria)
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
        public SopInstanceSpecificCharacterSet(DicomAttributeCollection criteria)
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