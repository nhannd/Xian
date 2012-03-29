using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal interface IPropertyFilter<T>
    {
        DicomTagPath Path { get; }
        DicomAttribute InputCriterion { get; }

        IQueryable<T> AddToQuery(IQueryable<T> inputQuery);
        IEnumerable<T> FilterResults(IEnumerable<T> results);

        void SetAttributeValue(T item, DicomAttributeCollection result);
    }

    internal abstract class PropertyFilter<T> : IPropertyFilter<T>
    {
        protected PropertyFilter(DicomTagPath path, IDicomAttributeProvider inputCriteria)
        {
            Path = path;
            InputCriterion = Path.GetAttribute(inputCriteria);
        }

        public DicomTagPath Path { get; private set; }
        public DicomAttribute InputCriterion { get; private set; }

        protected virtual bool IsCriterionValid
        {
            get { return !InputCriterion.IsEmpty && !InputCriterion.IsNull; }
        }

        public virtual IQueryable<T> AddToQuery(IQueryable<T> inputQuery)
        {
            return inputQuery;
        }

        public virtual IEnumerable<T> FilterResults(IEnumerable<T> results)
        {
            return results;
        }

        protected virtual void AddValueToResult(T item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetNullValue();
        }

        #region IPropertyFilter<T> Members

        IQueryable<T> IPropertyFilter<T>.AddToQuery(IQueryable<T> inputQuery)
        {
            if (!IsCriterionValid)
                return inputQuery;

            return AddToQuery(inputQuery);
        }

        IEnumerable<T> IPropertyFilter<T>.FilterResults(IEnumerable<T> results)
        {
            return FilterResults(results);
        }

        void IPropertyFilter<T>.SetAttributeValue(T item, DicomAttributeCollection result)
        {
            if (InputCriterion.IsEmpty)
                return;

            var resultAttribute = Path.GetAttribute(result, true);
            AddValueToResult(item, resultAttribute);
        }

        #endregion
    }
}