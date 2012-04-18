using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal class DicomPropertyFilter<TDatabaseObject> : PropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        protected DicomPropertyFilter(DicomTagPath path, IDicomAttributeProvider criteria)
        {
            Path = path;
            Criterion = Path.GetAttribute(criteria);
            IsReturnValueRequired = false;
            AddToQueryEnabled = true;
            FilterResultsEnabled = false;
        }

        public DicomTagPath Path { get; private set; }
        public DicomAttribute Criterion { get; private set; }

        protected internal override bool IsCriterionEmpty
        {
            get { return Criterion == null || Criterion.IsEmpty; }
        }

        protected internal override bool IsCriterionNull
        {
            get { return Criterion != null && Criterion.IsNull; }
        }

        protected internal override bool ShouldAddToQuery
        {
            get { return !IsCriterionEmpty && !IsCriterionNull; }
        }

        protected internal override bool ShouldAddToResult
        {
            get { return IsReturnValueRequired || !IsCriterionEmpty; }
        }

        protected virtual void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetNullValue();
        }

        protected sealed override void SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result)
        {
            var resultAttribute = Path.GetAttribute(result, true);
            AddValueToResult(item, resultAttribute);
        }

        public override string ToString()
        {
            return Path.ToString();
        }
    }
}