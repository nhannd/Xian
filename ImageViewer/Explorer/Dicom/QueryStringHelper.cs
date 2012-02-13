#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public static class QueryStringHelper
	{
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
				return String.Format("*{0}*", nameComponents[0].Trim());

			//Open name search - should never get here
			if (String.IsNullOrEmpty(nameComponents[0]))
				return String.Format("*{0}*", nameComponents[1].Trim());

				//Pure Last Name search
			if (String.IsNullOrEmpty(nameComponents[1]))
				return String.Format("{0}*", nameComponents[0].Trim());

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
