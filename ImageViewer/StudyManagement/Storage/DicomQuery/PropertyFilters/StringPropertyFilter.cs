using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class StringPropertyFilter<T> : PropertyFilter<T>
    {
        //These are the VRs DICOM says can't be searched on with wildcards,
        //therefore any wildcard characters present in the criteria are literal.
        private static readonly string[] WildcardExcludedVRs = { "DA", "TM", "DT", "SL", "SS", "US", "UL", "FL", "FD", "OB", "OW", "UN", "AT", "DS", "IS", "AS", "UI" };

        private bool _parsedCriterion;
        private string _criterionValue;

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

        public string CriterionValue
        {
            get
            {
                if (!_parsedCriterion)
                {
                    _criterionValue = Criterion.GetString(0, null);
                    if (_criterionValue != null)
                    {
                        //These are the Linq To SQL wildcard characters.
                        _criterionValue = _criterionValue.Replace("*", "%").Replace("?", "_");
                    }
                }

                return _criterionValue;
            }
        }

        protected internal bool IsCriterionWildcard
        {
            get
            {
                if (!IsWildcardCriterionAllowed)
                    return false;

                return CriterionValue.Contains("%") || CriterionValue.Contains("_");
            }
        }

        protected internal bool IsWildcardCriterionAllowed
        {
            get { return !WildcardExcludedVRs.Any(excludedVr => excludedVr == Path.ValueRepresentation.Name); }    
        }
    }
}
