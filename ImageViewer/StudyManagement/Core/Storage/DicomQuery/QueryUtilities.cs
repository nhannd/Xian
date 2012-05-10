using System;
using System.Linq;
using System.Text.RegularExpressions;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery
{
    internal static class QueryUtilities
    {
        //These are the VRs DICOM says can't be searched on with wildcards,
        //therefore any wildcard characters present in the criteria are literal.
        private static readonly string[] WildcardExcludedVRs = { "DA", "TM", "DT", "SL", "SS", "US", "UL", "FL", "FD", "OB", "OW", "UN", "AT", "DS", "IS", "AS", "UI" };

        internal static bool IsWildcardCriterionAllowed(DicomVr vr)
        {
            return !WildcardExcludedVRs.Any(excludedVr => excludedVr == vr.Name);
        }

        internal static bool IsWildcardCriterion(DicomVr vr, string criterion)
        {
            if (!IsWildcardCriterionAllowed(vr))
                return false;

            if (String.IsNullOrEmpty(criterion))
                return false;

            return criterion.Contains("*") || criterion.Contains("?");
        }

        internal static bool IsLike(string value, string criterion)
        {
            string test = criterion.Replace("*", ".*"); //zero or more characters
            test = test.Replace("?", "."); //single character
            test = String.Format("^{0}", test); //match at beginning

            //DICOM says if we manage an object having no value, it's considered a match.
            return String.IsNullOrEmpty(value)
                //DICOM says matching is case sensitive, but that's just silly.
                   || Regex.IsMatch(value, test, RegexOptions.IgnoreCase);
        }

        internal static bool AreEqual(string value, string criterion)
        {
            //DICOM says if we manage an object having no value, it's considered a match.
            return String.IsNullOrEmpty(value)
                //DICOM says matching is case sensitive, but that's just silly.
                || 0 == string.Compare(value, criterion, StringComparison.InvariantCultureIgnoreCase);
        }

        internal static bool IsMultiValued(string value)
        {
            return !String.IsNullOrEmpty(value) && value.Contains(@"\");
        }
    }
}
