using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    /// <summary>
    /// Base class for <see cref="PropertyFilter{TDatabaseObject}"/>s.
    /// </summary>
    /// <remarks><see cref="DicomPropertyFilter{TDatabaseObject}"/>s use the template and rule
    /// design patterns to allow subclasses to implement only what they need to, and not have
    /// to worry about providing any logic. Subclasses should only have to filter SQL queries
    /// and return property values for post-filtering.</remarks>
    internal interface IPropertyFilter<TDatabaseObject> 
        where TDatabaseObject : class 
    {
        IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query);
        IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results);
        void SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result);
    }

    internal abstract class PropertyFilter<TDatabaseObject> : IPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        protected internal bool AddToQueryEnabled { get; set; }
        protected internal bool FilterResultsEnabled { get; set; }

        protected internal abstract bool ShouldAddToQuery { get; }
        protected internal virtual bool ShouldAddToResult { get; set; }

        protected virtual IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            return query;
        }

        protected virtual IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results)
        {
            return results;
        }

        protected abstract void SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result);

        #region IPropertyFilter<TDatabaseObject> Members

        IQueryable<TDatabaseObject> IPropertyFilter<TDatabaseObject>.AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (!AddToQueryEnabled || !ShouldAddToQuery)
                return query;

            return AddToQuery(query);
        }

        IEnumerable<TDatabaseObject> IPropertyFilter<TDatabaseObject>.FilterResults(IEnumerable<TDatabaseObject> results)
        {
            if (!FilterResultsEnabled || !ShouldAddToQuery)
                return results;

            return FilterResults(results);
        }

        void IPropertyFilter<TDatabaseObject>.SetAttributeValue(TDatabaseObject item, DicomAttributeCollection result)
        {
            if (ShouldAddToResult)
                SetAttributeValue(item, result);
        }

        #endregion
    }
}