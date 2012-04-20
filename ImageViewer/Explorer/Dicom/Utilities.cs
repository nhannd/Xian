using System;
using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.Automation;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    internal static class Utilities
    {
        public static bool IsOpenQuery(this StudyRootStudyIdentifier criteria)
        {
            var attributes = criteria.ToDicomAttributeCollection();

            //Clean out the base identifier attributes, which could have non-empty values.
            attributes[DicomTags.SpecificCharacterSet].SetEmptyValue();
            attributes[DicomTags.RetrieveAeTitle].SetEmptyValue();
            attributes[DicomTags.InstanceAvailability].SetEmptyValue();
            attributes[DicomTags.QueryRetrieveLevel].SetEmptyValue();

            return attributes.Any(a => !a.IsNull && !a.IsEmpty);
        }

        public static StudyRootStudyIdentifier ToIdentifier(this DicomExplorerSearchCriteria explorerSearchCriteria, bool addWildcards)
        {
            if (addWildcards)
            {
                return new StudyRootStudyIdentifier
                {
                    PatientsName = ConvertNameToSearchCriteria(explorerSearchCriteria.PatientsName),
                    ReferringPhysiciansName = ConvertNameToSearchCriteria(explorerSearchCriteria.ReferringPhysiciansName),
                    PatientId = ConvertStringToWildcardSearchCriteria(explorerSearchCriteria.PatientId, false, true),
                    AccessionNumber = ConvertStringToWildcardSearchCriteria(explorerSearchCriteria.AccessionNumber, false, true),
                    StudyDescription = ConvertStringToWildcardSearchCriteria(explorerSearchCriteria.StudyDescription, false, true),
                    StudyDate = DateRangeHelper.GetDicomDateRangeQueryString(explorerSearchCriteria.StudyDateFrom, explorerSearchCriteria.StudyDateTo),
                    //At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
                    //Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
                    //underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
                    //or by running multiple queries, one per modality specified (for example).
                    ModalitiesInStudy = explorerSearchCriteria.Modalities.ToArray()
                };
            }

            return new StudyRootStudyIdentifier
            {
                PatientsName = explorerSearchCriteria.PatientsName,
                ReferringPhysiciansName = explorerSearchCriteria.ReferringPhysiciansName,
                PatientId = explorerSearchCriteria.PatientId,
                AccessionNumber = explorerSearchCriteria.AccessionNumber,
                StudyDescription = explorerSearchCriteria.StudyDescription,
                StudyDate = DateRangeHelper.GetDicomDateRangeQueryString(explorerSearchCriteria.StudyDateFrom, explorerSearchCriteria.StudyDateTo),
                //At the application level, ClearCanvas defines the 'ModalitiesInStudy' filter as a multi-valued
                //Key Attribute.  This goes against the Dicom standard for C-FIND SCU behaviour, so the
                //underlying IStudyFinder(s) must handle this special case, either by ignoring the filter
                //or by running multiple queries, one per modality specified (for example).
                ModalitiesInStudy = explorerSearchCriteria.Modalities.ToArray()
            };
        }

        /// <summary>
        /// Converts the query string into a DICOM search criteria.
        /// Appended with a wildcard character.
        /// </summary>
        public static string ConvertStringToWildcardSearchCriteria(string userQueryString, bool leadingWildcard, bool trailingWildcard)
        {
            var dicomSearchCriteria = "";
            if (String.IsNullOrEmpty(userQueryString))
                return dicomSearchCriteria;

            dicomSearchCriteria = userQueryString;
            if (leadingWildcard)
                dicomSearchCriteria = "*" + dicomSearchCriteria;

            if (trailingWildcard)
                dicomSearchCriteria = dicomSearchCriteria + "*";

            return dicomSearchCriteria;
        }

        /// <summary>
        /// Converts the query string for name into a DICOM search string.
        /// </summary>
        public static string ConvertNameToSearchCriteria(string name)
        {
            var nameComponents = GetNameComponents(name);

            if (nameComponents.Length == 0)
                return "";

            //Open name search
            if (nameComponents.Length == 1)
                return String.Format("*{0}*", (object) nameComponents[0].Trim());

            //Open name search - should never get here
            if (String.IsNullOrEmpty(nameComponents[0]))
                return String.Format("*{0}*", (object) nameComponents[1].Trim());

            //Pure Last Name search
            if (String.IsNullOrEmpty(nameComponents[1]))
                return String.Format("{0}*", (object) nameComponents[0].Trim());

            //Last Name, First Name search
            return String.Format("{0}*{1}*", nameComponents[0].Trim(), nameComponents[1].Trim());
        }

        private static string[] GetNameComponents(string unparsedName)
        {
            unparsedName = unparsedName ?? "";
            var separator = DicomExplorerConfigurationSettings.Default.NameSeparator;
            var name = unparsedName.Trim();
            if (String.IsNullOrEmpty(name))
                return new string[0];

            return name.Split(new[] { separator }, StringSplitOptions.None);
        }
    }
}
