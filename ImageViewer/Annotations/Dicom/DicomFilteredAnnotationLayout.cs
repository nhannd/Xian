#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	internal sealed class DicomFilteredAnnotationLayout
	{
		private readonly string _identifier;
		private readonly string _matchingLayoutIdentifier;
		private readonly List<KeyValuePair<string, string>> _filters;

		public DicomFilteredAnnotationLayout(string identifier, string matchingLayoutIdentifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(matchingLayoutIdentifier, "matchingLayoutIdentifier");

			_identifier = identifier;
			_matchingLayoutIdentifier = matchingLayoutIdentifier;

			_filters = new List<KeyValuePair<string, string>>();
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public string MatchingLayoutIdentifier
		{
			get { return _matchingLayoutIdentifier; }
		}

		public IList<KeyValuePair<string, string>> Filters
		{
			get { return _filters; }
		}

		internal bool IsMatch(List<KeyValuePair<string, string>> filterCandidates)
		{
			foreach (KeyValuePair<string, string> filter in _filters)
			{
				foreach (KeyValuePair<string, string> candidate in filterCandidates)
				{
					if (candidate.Key == filter.Key)
					{
						if (filter.Value != candidate.Value)
							return false;
					}
				}
			}

			//all filters matched, or there were none, which is always a match.
			return true;
		}
	}
}
