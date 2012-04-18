using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal abstract class StringDicomPropertyFilter<TDatabaseObject> 
        : DicomPropertyFilter<TDatabaseObject>
        , IMultiValuedPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        private string[] _criterionValues;
        private readonly MultiValuedPropertyRule<TDatabaseObject> _rule;

        protected StringDicomPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria) 
            : base(path, criteria)
        {
            _rule = new MultiValuedPropertyRule<TDatabaseObject>(this);
        }

        protected StringDicomPropertyFilter(DicomTag tag, DicomAttributeCollection criteria)
            : this(new DicomTagPath(tag), criteria)
        {
        }

        protected StringDicomPropertyFilter(uint tag, DicomAttributeCollection criteria)
            : this(new DicomTagPath(tag), criteria)
        {
        }

        string[] IMultiValuedPropertyFilter<TDatabaseObject>.CriterionValues
        {
            get 
            {
                var criterionValue = Criterion == null ? String.Empty : Criterion.ToString();
                return _criterionValues ?? (_criterionValues = DicomStringHelper.GetStringArray(criterionValue) ?? new string[0]);
            }
        }

        bool IMultiValuedPropertyFilter<TDatabaseObject>.IsWildcardCriterionAllowed
        {
            get { return QueryUtilities.IsWildcardCriterionAllowed(Path.ValueRepresentation); }
        }

        bool IMultiValuedPropertyFilter<TDatabaseObject>.IsWildcardCriterion(string criterion)
        {
            return QueryUtilities.IsWildcardCriterion(Path.ValueRepresentation, criterion);
        }

        IQueryable<TDatabaseObject> IMultiValuedPropertyFilter<TDatabaseObject>.AddEqualsToQuery(IQueryable<TDatabaseObject> query, string criterion)
        {
            return AddEqualsToQuery(query, criterion);
        }

        IQueryable<TDatabaseObject> IMultiValuedPropertyFilter<TDatabaseObject>.AddLikeToQuery(IQueryable<TDatabaseObject> query, string criterion)
        {
            return AddLikeToQuery(query, criterion);
        }

        protected virtual string GetPropertyValue(TDatabaseObject item)
        {
            throw new NotImplementedException("GetPropertyValue must be overridden to do post-filtering.");
        }

        string[] IMultiValuedPropertyFilter<TDatabaseObject>.GetPropertyValues(TDatabaseObject item)
        {
            return DicomStringHelper.GetStringArray(GetPropertyValue(item));
        }

        protected override IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            return _rule.AddToQuery(query);
        }

        protected override IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results)
        {
            return _rule.FilterResults(results);
        }

        protected virtual IQueryable<TDatabaseObject> AddEqualsToQuery(IQueryable<TDatabaseObject> query, string criterion)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, AddEqualsToQuery must be implemented.");
        }

        protected virtual IQueryable<TDatabaseObject> AddLikeToQuery(IQueryable<TDatabaseObject> query, string criterion)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, AddLikeToQuery must be implemented.");
        }
    }
}
