using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal abstract class StringPropertyFilter<TDatabaseObject, TStoreEntry> : DicomPropertyFilter<TDatabaseObject, TStoreEntry>
        where TDatabaseObject : class
        where TStoreEntry : StoreEntry
    {
        //These are the VRs DICOM says can't be searched on with wildcards,
        //therefore any wildcard characters present in the criteria are literal.
        private static readonly string[] WildcardExcludedVRs = { "DA", "TM", "DT", "SL", "SS", "US", "UL", "FL", "FD", "OB", "OW", "UN", "AT", "DS", "IS", "AS", "UI" };

        private string[] _criterionValues;

        protected StringPropertyFilter(DicomTagPath path, DicomAttributeCollection criteria) 
            : base(path, criteria)
        {
        }

        protected StringPropertyFilter(DicomTag tag, DicomAttributeCollection criteria)
            : this(new DicomTagPath(tag), criteria)
        {
        }

        protected StringPropertyFilter(uint tag, DicomAttributeCollection criteria)
            : this(new DicomTagPath(tag), criteria)
        {
        }

        protected string[] CriterionValues
        {
            get 
            {
                return _criterionValues ??
                    (_criterionValues = DicomStringHelper.GetStringArray(CriterionValue) ?? new string[0]);
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
            get { return !WildcardExcludedVRs.Any(excludedVr => excludedVr == Path.ValueRepresentation.Name); }    
        }

        internal bool IsWildcardCriterion(string criterion)
        {
            if (!IsWildcardCriterionAllowed)
                return false;

            if (String.IsNullOrEmpty(criterion))
                return false;

            return criterion.Contains("*") || criterion.Contains("?");
        }

        internal bool IsMultiValued(string value)
        {
            return !String.IsNullOrEmpty(value) && value.Contains(@"\");
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

        protected bool AreEqual(string value, string criterion)
        {
            //DICOM says if we manage an object having no value, it's considered a match.
            return String.IsNullOrEmpty(value)
            //DICOM says matching is case sensitive, but that's just silly.
                || 0 == string.Compare(value, criterion, StringComparison.InvariantCultureIgnoreCase);
        }

        protected bool IsLike(string value, string criterion)
        {
            string test = criterion.Replace("*", ".*"); //zero or more characters
            test = test.Replace("?", "."); //single character
            test = String.Format("^{0}", test); //match at beginning

            //DICOM says if we manage an object having no value, it's considered a match.
            return String.IsNullOrEmpty(value)
                   //DICOM says matching is case sensitive, but that's just silly.
                   || Regex.IsMatch(value, test, RegexOptions.IgnoreCase);
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
                return propertyValues.Any(value => AreEqual(value, criterion));

            return propertyValues.Any(value => IsLike(value, criterion));
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
