using System;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.PropertyFilters
{
    internal class StringPropertyFilter<T> : PropertyFilter<T>
    {
        private static readonly string[] WildcardExcludedVRs = { "DA", "TM", "DT", "SL", "SS", "US", "UL", "FL", "FD", "OB", "OW", "UN", "AT", "DS", "IS", "AS", "UI" };

        protected StringPropertyFilter(DicomTagPath path, DicomAttributeCollection inputCriteria) 
            : base(path, inputCriteria)
        {
        }

        protected StringPropertyFilter(DicomTag tag, DicomAttributeCollection inputCriteria)
            : this(new DicomTagPath(tag), inputCriteria)
        {
        }

        protected StringPropertyFilter(uint tag, DicomAttributeCollection inputCriteria)
            : this(new DicomTagPath(tag), inputCriteria)
        {
        }

        //protected override bool IsCriterionValid
        //{
        //    get { return !String.IsNullOrEmpty(CriterionValue) && base.IsCriterionValid; }
        //}

        protected bool IsCriterionWildcard
        {
            get
            {
                if (!IsWildcardCriterionAllowed)
                    return false;

                return ContainsWildcardCharacters(InputCriterion);
            }
        }

        protected bool IsWildcardCriterionAllowed
        {
            get 
            {
                foreach (string excludedVR in WildcardExcludedVRs)
                {
                    if (0 == String.Compare(excludedVR, Path.ValueRepresentation.Name, true))
                        return false;
                }

                return true;
            }    
        }

        public string CriterionValue
        {
            get
            {
                var criterionValue = InputCriterion.GetString(0, null);
                if (criterionValue == null)
                    return null;

                //TODO (Marmot): store the value.
                return criterionValue.Replace("*", "%").Replace("?", "_");
            }    
        }

        private static bool ContainsWildcardCharacters(string criteria)
        {
            return criteria.Contains("*") || criteria.Contains("?");
        }
    }
}
