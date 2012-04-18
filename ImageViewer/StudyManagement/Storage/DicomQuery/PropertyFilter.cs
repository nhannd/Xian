using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal interface IPropertyFilter<TDatabaseObject, in TStoreEntry> 
        where TDatabaseObject : class 
        where TStoreEntry : StoreEntry
    {
        //DicomTagPath Path { get; }
        //DicomAttribute Criterion { get; }

        IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query);
        IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results);

        void SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result);
    }

    internal class DicomPropertyFilter<TDatabaseObject, TStudyStoreEntry> : IPropertyFilter<TDatabaseObject, TStudyStoreEntry>
        where TDatabaseObject : class
        where TStudyStoreEntry : StoreEntry
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

        /// <summary>
        /// The value is required to be returned in the results.
        /// </summary>
        protected internal bool IsReturnValueRequired { get; set; }
        protected internal bool AddToQueryEnabled { get; set; }
        protected internal bool FilterResultsEnabled { get; set; }

        protected internal virtual bool IsCriterionEmpty
        {
            get { return Criterion == null || Criterion.IsEmpty; }
        }

        protected internal virtual bool IsCriterionNull
        {
            get { return Criterion != null && Criterion.IsNull; }
        }

        protected internal virtual bool ShouldAddToQuery
        {
            get { return !IsCriterionEmpty && !IsCriterionNull; }
        }

        protected internal virtual bool ShouldAddToResult
        {
            get { return IsReturnValueRequired || !IsCriterionEmpty; }
        }

        protected virtual IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            return query;
        }

        protected virtual IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results)
        {
            return results;
        }

        protected virtual void AddValueToResult(TDatabaseObject item, DicomAttribute resultAttribute)
        {
            resultAttribute.SetNullValue();
        }

        #region IPropertyFilter<TDatabaseObject> Members

        IQueryable<TDatabaseObject> IPropertyFilter<TDatabaseObject, TStudyStoreEntry>.AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (!AddToQueryEnabled || !ShouldAddToQuery)
                return query;

            return AddToQuery(query);
        }

        IEnumerable<TDatabaseObject> IPropertyFilter<TDatabaseObject, TStudyStoreEntry>.FilterResults(IEnumerable<TDatabaseObject> results)
        {
            if (!FilterResultsEnabled || !ShouldAddToQuery)
                return results;

            return FilterResults(results);
        }

        void IPropertyFilter<TDatabaseObject, TStudyStoreEntry>.SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result)
        {
            if (!ShouldAddToResult)
                return;

            var resultAttribute = Path.GetAttribute(result, true);
            AddValueToResult(item, resultAttribute);
        }

        #endregion

        public override string ToString()
        {
            return Path.ToString();
        }
    }
}