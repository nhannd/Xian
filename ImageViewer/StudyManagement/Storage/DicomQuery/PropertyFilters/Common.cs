using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    #region InstanceAvailability

    internal class InstanceAvailability<TDatabaseObject, TStoreEntry> : DicomPropertyFilter<TDatabaseObject, TStoreEntry>
        where TDatabaseObject : class
        where TStoreEntry : StoreEntry, IStoreEntry
    {
        public InstanceAvailability(IDicomAttributeProvider criteria)
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

    internal class StudyInstanceAvailability : InstanceAvailability<Study, StudyEntry>
    {
        public StudyInstanceAvailability(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    internal class SeriesInstanceAvailability : InstanceAvailability<Series, SeriesEntry>
    {
        public SeriesInstanceAvailability(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceAvailability : InstanceAvailability<SopInstance, ImageEntry>
    {
        public SopInstanceAvailability(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region RetrieveAETitle

    internal class RetrieveAETitle<TDatabaseObject, TStoreEntry> : DicomPropertyFilter<TDatabaseObject, TStoreEntry>
        where TDatabaseObject : class
        where TStoreEntry : StoreEntry, IStoreEntry
    {
        public RetrieveAETitle(IDicomAttributeProvider criteria)
            : base(DicomTags.RetrieveAeTitle, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = false;
            base.IsReturnValueRequired = true;
        }

        protected override void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, Utilities.GetLocalServerAETitle());
        }
    }

    internal class StudyRetrieveAETitle: RetrieveAETitle<Study, StudyEntry>
    {
        public StudyRetrieveAETitle(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    internal class SeriesRetrieveAETitle : RetrieveAETitle<Series, SeriesEntry>
    {
        public SeriesRetrieveAETitle(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    internal class SopInstanceRetrieveAETitle : RetrieveAETitle<SopInstance, ImageEntry>
    {
        public SopInstanceRetrieveAETitle(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }
    }

    #endregion

    #region Specific Character Set

    internal abstract class SpecificCharacterSet<TDatabaseObject, TStoreEntry> : DicomPropertyFilter<TDatabaseObject, TStoreEntry>
        where TDatabaseObject : class
        where TStoreEntry : StoreEntry
    {
        protected SpecificCharacterSet(IDicomAttributeProvider criteria)
            : base(DicomTags.SpecificCharacterSet, criteria)
        {
            base.AddToQueryEnabled = false;
            base.FilterResultsEnabled = false;
            base.IsReturnValueRequired = true;
        }
    }

    internal class StudySpecificCharacterSet : SpecificCharacterSet<Study, StudyEntry>
    {
        public StudySpecificCharacterSet(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(Study item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, item.SpecificCharacterSet);
        }
    }

    internal class SeriesSpecificCharacterSet : SpecificCharacterSet<Series, SeriesEntry>
    {
        public SeriesSpecificCharacterSet(IDicomAttributeProvider criteria)
            : base(criteria)
        {
        }

        protected override void AddValueToResult(Series item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetString(0, item.SpecificCharacterSet);
        }
    }

    internal class SopInstanceSpecificCharacterSet : SpecificCharacterSet<SopInstance, ImageEntry>
    {
        public SopInstanceSpecificCharacterSet(IDicomAttributeProvider criteria)
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