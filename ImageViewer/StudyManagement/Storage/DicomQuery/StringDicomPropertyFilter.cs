using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal abstract class StringDicomPropertyFilter<TDatabaseObject> : DicomPropertyFilter<TDatabaseObject>
        where TDatabaseObject : class
    {
        private string[] _criterionValues;

        protected StringDicomPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria) 
            : base(path, criteria)
        {
        }

        protected StringDicomPropertyFilter(DicomTag tag, DicomAttributeCollection criteria)
            : this(new DicomTagPath(tag), criteria)
        {
        }

        protected StringDicomPropertyFilter(uint tag, DicomAttributeCollection criteria)
            : this(new DicomTagPath(tag), criteria)
        {
        }

        protected string[] CriterionValues
        {
            get 
            {
                return _criterionValues ?? (_criterionValues = DicomStringHelper.GetStringArray(CriterionValue) ?? new string[0]);
            }
        }

        protected internal string CriterionValue
        {
            get
            {
                return Criterion == null ? String.Empty : Criterion.ToString();
            }
        }

        protected internal bool IsWildcardCriterionAllowed
        {
            get { return QueryUtilities.IsWildcardCriterionAllowed(Path.ValueRepresentation); }
        }

        protected internal bool IsWildcardCriterion(string criterion)
        {
            return QueryUtilities.IsWildcardCriterion(Path.ValueRepresentation, criterion);
        }

        protected sealed override IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query)
        {
            if (CriterionValues.Length > 1)
                return AddToQuery(query, CriterionValues);

            return AddToQuery(query, CriterionValue);
        }

        protected IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> query, string criterionValue)
        {
            if (!IsWildcardCriterion(criterionValue))
                return AddEqualsToQuery(query, criterionValue);

            var sqlCriterion = criterionValue.Replace("*", "%").Replace("?", "_");
            var returnQuery = AddLikeToQuery(query, sqlCriterion);
            return returnQuery;
        }

        protected IQueryable<TDatabaseObject> AddToQuery(IQueryable<TDatabaseObject> inputQuery, string[] criterionValues)
        {
            IQueryable<TDatabaseObject> unionedQuery = null;
            foreach (var criterionValue in criterionValues.Where(value => !String.IsNullOrEmpty(value)))
            {
                var criterionQuery = AddToQuery(inputQuery, criterionValue);
                unionedQuery = unionedQuery == null ? criterionQuery : unionedQuery.Union(criterionQuery);
            }

            return unionedQuery ?? inputQuery;
        }

        protected virtual IQueryable<TDatabaseObject> AddEqualsToQuery(IQueryable<TDatabaseObject> query, string criterion)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, this must be implemented.");
        }

        protected virtual IQueryable<TDatabaseObject> AddLikeToQuery(IQueryable<TDatabaseObject> query, string criterion)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, this must be implemented.");
        }

        protected virtual string GetPropertyValue(TDatabaseObject item)
        {
            throw new NotImplementedException("GetPropertyValue must be overridden to do post-filtering.");
        }

        protected bool IsMatch(TDatabaseObject result, string criterion)
        {
            var propertyValue = GetPropertyValue(result);
            if (String.IsNullOrEmpty(propertyValue))
            {
                //DICOM says if we maintain an object with an empty value, it's a match for any criteria.
                return true;
            }

            var propertyValues = DicomStringHelper.GetStringArray(propertyValue);

            if (!IsWildcardCriterion(criterion))
                return propertyValues.Any(value => QueryUtilities.AreEqual(value, criterion));

            return propertyValues.Any(value => QueryUtilities.IsLike(value, criterion));
        }

        protected IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results, string[] criterionValues)
        {
            var resultsList = new List<TDatabaseObject>(results);
            IEnumerable<TDatabaseObject> unionedResults = null;
            foreach (var criterionValue in criterionValues)
            {
                var criterion = criterionValue;
                var criterionResults = resultsList.Where(result => IsMatch(result, criterion));
                unionedResults = unionedResults == null ? criterionResults : unionedResults.Union(criterionResults);
            }

            return unionedResults ?? results;
        }

        protected IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results, string criterionValue)
        {
            //DICOM says if we maintain an object with an empty value, it's a match for any criteria.
            if (string.IsNullOrEmpty(criterionValue))
                return results;

            return results.Where(result => IsMatch(result, criterionValue));
        }

        protected override IEnumerable<TDatabaseObject> FilterResults(IEnumerable<TDatabaseObject> results)
        {
            if (CriterionValues.Length > 1)
            {
                var filtered = FilterResults(results, CriterionValues);
                return filtered;
            }
            else
            {
                var filtered = FilterResults(results, CriterionValue);
                return filtered;
            }
        }
    }
}
