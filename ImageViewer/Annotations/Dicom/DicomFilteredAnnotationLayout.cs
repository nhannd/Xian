using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	internal sealed class DicomFilteredAnnotationLayout
	{
		private string _identifier;
		private string _matchingLayoutIdentifier;
		private List<KeyValuePair<string, string>> _filters;

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
