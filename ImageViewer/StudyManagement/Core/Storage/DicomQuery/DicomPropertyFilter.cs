using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery
{
    /// <summary>
    /// Base class for <see cref="PropertyFilter{TDatabaseObject}"/>s that are 1:1 with a DICOM Attribute
    /// that can be queried and returned according to Part 4 of the DICOM standard.
    /// </summary>
    /// <remarks><see cref="DicomPropertyFilter{TDatabaseObject}"/>s use the template and rule
    /// design patterns to allow subclasses to implement only what they need to, and not have
    /// to worry about providing any logic. Subclasses should only have to filter SQL queries
    /// and return property values for post-filtering.</remarks>
    internal abstract class DicomPropertyFilter<TDatabaseObject> : PropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        protected DicomPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria)
        {
            Path = path;
            Criterion = Path.GetAttribute(criteria);
            IsReturnValueRequired = false;
            AddToQueryEnabled = true;
            FilterResultsEnabled = false;
        }

        protected internal bool IsReturnValueRequired { get; set; }

        public DicomTagPath Path { get; private set; }
        public DicomAttribute Criterion { get; private set; }

        protected internal bool IsCriterionEmpty
        {
            get { return Criterion == null || Criterion.IsEmpty; }
        }

        protected internal bool IsCriterionNull
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