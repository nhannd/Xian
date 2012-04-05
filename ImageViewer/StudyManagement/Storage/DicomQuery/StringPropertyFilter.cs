using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    internal abstract class StringPropertyFilter<T> : PropertyFilter<T>
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
                if (Criterion == null)
                    return String.Empty;

                return Criterion.ToString();
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

        protected sealed override IQueryable<T> AddToQuery(IQueryable<T> query)
        {
            if (CriterionValues.Length > 1)
                return AddToQuery(query, CriterionValues);

            return AddToQuery(query, CriterionValue);
        }

        protected IQueryable<T> AddToQuery(IQueryable<T> query, string criterionValue)
        {
            if (!IsWildcardCriterion(criterionValue))
                return AddEqualsToQuery(query, criterionValue);

            var sqlCriterion = criterionValue.Replace("*", "%").Replace("?", "_");
            var returnQuery = AddLikeToQuery(query, sqlCriterion);
            return returnQuery;
        }

        protected IQueryable<T> AddToQuery(IQueryable<T> query, string[] criterionValues)
        {
            IQueryable<T> unionedQuery = null;
            foreach (var criterionValue in criterionValues.Where(v => !String.IsNullOrEmpty(v)))
            {
                var criterionQuery = AddToQuery(query, criterionValue);
                unionedQuery = unionedQuery == null ? criterionQuery : unionedQuery.Union(criterionQuery);
            }

            return unionedQuery ?? query;
        }

        protected virtual IQueryable<T> AddEqualsToQuery(IQueryable<T> query, string criterion)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, this must be implemented.");
        }

        protected virtual IQueryable<T> AddLikeToQuery(IQueryable<T> query, string criterion)
        {
            throw new NotImplementedException("If AddToQueryEnabled is true, this must be implemented.");
        }

        protected virtual string GetPropertyValue(T item)
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

        protected bool IsMatch(T result, string criterion)
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

        protected IEnumerable<T> FilterResults(IEnumerable<T> results, string[] criterionValues)
        {
            var resultsList = new List<T>(results);
            IEnumerable<T> unionedResults = null;
            foreach (var criterionValue in criterionValues)
            {
                var c = criterionValue;
                var criterionResults = resultsList.Where(result => IsMatch(result, c));
                unionedResults = unionedResults == null ? criterionResults : unionedResults.Union(criterionResults);
            }

            return unionedResults ?? results;
        }

        protected IEnumerable<T> FilterResults(IEnumerable<T> results, string criterionValue)
        {
            if (string.IsNullOrEmpty(criterionValue))
                return results;

            return results.Where(result => IsMatch(result, criterionValue));
        }

        protected override IEnumerable<T> FilterResults(IEnumerable<T> results)
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
