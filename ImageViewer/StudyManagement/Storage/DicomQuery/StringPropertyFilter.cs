using System;
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

        protected string CriterionValue
        {
            get { return Criterion.GetString(0, null); }
        }

        protected internal bool IsCriterionWildcard
        {
            get
            {
                if (IsCriterionEmpty || IsCriterionNull)
                    return false;

                if (!IsWildcardCriterionAllowed)
                    return false;

                return CriterionValue.Contains("*") || CriterionValue.Contains("?");
            }
        }

        protected internal bool IsWildcardCriterionAllowed
        {
            get { return !WildcardExcludedVRs.Any(excludedVr => excludedVr == Path.ValueRepresentation.Name); }    
        }

        protected sealed override IQueryable<T> AddToQuery(IQueryable<T> query)
        {
            if (!IsCriterionWildcard)
                return AddEqualsToQuery(query, CriterionValue);

            var sqlCriterion = CriterionValue.Replace("*", "%").Replace("?", "_");
            return AddLikeToQuery(query, sqlCriterion);
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

        protected override System.Collections.Generic.IEnumerable<T> FilterResults(System.Collections.Generic.IEnumerable<T> results)
        {
            if (!IsCriterionWildcard)
            {
                //Note: single value matching is supposed to be case-sensitive (except for PN) according to Dicom.
                return results.Where(result => 0 == string.Compare(GetPropertyValue(result), CriterionValue, StringComparison.InvariantCultureIgnoreCase));
            }

            //Wildcard on strings is generic.
            string criteriaTest = CriterionValue.Replace("*", ".*"); //zero or more characters
            criteriaTest = criteriaTest.Replace("?", "."); //single character
            criteriaTest = String.Format("^{0}", criteriaTest); //match at beginning

            return results.Where(result => Regex.IsMatch(GetPropertyValue(result), criteriaTest, RegexOptions.IgnoreCase));
        }
    }
}
