using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ClearCanvas.DicomServices;

namespace ClearCanvas.Dicom.DataStore
{
	internal class QueryResultFilter<T>
	{
		private readonly QueryCriteria _queryCriteria;
		private readonly IEnumerable<T> _resultCandidates;

		public QueryResultFilter(QueryCriteria queryCriteria, IEnumerable<T> resultCandidates)
		{
			_queryCriteria = queryCriteria;
			_resultCandidates = resultCandidates;
		}

		public List<DicomAttributeCollection> GetResults()
		{
			List<DicomAttributeCollection> results = new List<DicomAttributeCollection>();
			foreach (T candidate in _resultCandidates)
			{
				DicomAttributeCollection result = GetResult(candidate);
				if (result != null)
					results.Add(result);
			}

			return results;
		}

		private DicomAttributeCollection GetResult(T candidate)
		{
			QueryablePropertyCollection<T> queryableProperties = new QueryablePropertyCollection<T>();
			DicomAttributeCollection result = new DicomAttributeCollection();

			foreach (QueryablePropertyInfo property in queryableProperties)
			{
				string criteria = _queryCriteria[property.Path];
				if (criteria == null && !property.ReturnAlways)
					continue;

				object value = property.ReturnProperty.GetValue(candidate, null);
				if (value == null && !property.ReturnAlways)
					continue;

				string[] values = PropertyValueToStringArray(value);
				if (criteria != null && property.IsComputed && !property.ReturnOnly)
				{
					//universal matching (empty criteria = all match)
					if (!String.IsNullOrEmpty(criteria))
					{
						string[] criteriaValues = CriteriaValueToStringArray(property, criteria);
						if (!AnyMatch(property, values, criteriaValues))
							return null;
					}
				}

				string propertyValue = FormatPropertyValuesForResult(values);
				AddValueToResult(property.Path, propertyValue, result);
			}

			return result;
		}

		private static bool AnyMatch(QueryablePropertyInfo property, IEnumerable<string> testValues, IEnumerable<string> criteriaValues)
		{
			foreach (string criteria in criteriaValues)
			{
				foreach (string test in testValues)
				{
					// Note: right now, the only computed properties are of properties that undergo
					// these types of matching.  If more are added (dates, sequence matching, for example) then code will
					// need to be added here.

					if (property.Path.ValueRepresentation == DicomVr.UIvr) //list of uid matching.
					{
						//any matching uid is considered a match.
						if (test.Equals(criteria))
							return true;
					}
					else if (criteria.Contains("*") || criteria.Contains("?")) // wildcard matching.
					{
						string criteriaTest = criteria.Replace("*", "[\x21-\x7E]").Replace("?", ".");
						//a match on any of the values is considered a match.
						if (Regex.IsMatch(test, criteriaTest))
							return true;
					}
					else if (criteria.Equals(test)) //single value matching.
						return true;
				}
			}

			return false;
		}

		private static string[] PropertyValueToStringArray(object value)
		{
			if (value is string)
				return new string[] { value as string };
			else if (value is string[])
				return value as string[];
			else if (value is int)
				return new string[] { ((int)value).ToString() };

			throw new InvalidOperationException("Proper conversion code must be written.");
		}

		private static string[] CriteriaValueToStringArray(QueryablePropertyInfo property, string criteria)
		{
			if (property.AllowListMatching)
				return DicomStringHelper.GetStringArray(criteria);
			else
				return new string[] { criteria };
		}

		private static string FormatPropertyValuesForResult(string[] values)
		{
			if (values == null || values.Length == 0)
				return "";

			return DicomStringHelper.GetDicomStringArray(values) ?? "";
		}

		private static void AddValueToResult(DicomTagPath atrributePath, string value, DicomAttributeCollection result)
		{
			DicomAttribute attribute = AddAttributeToResult(atrributePath, result);
			attribute.SetStringValue(value);
		}

		private static DicomAttribute AddAttributeToResult(DicomTagPath atrributePath, DicomAttributeCollection result)
		{
			int i = 0;
			while (i < atrributePath.TagsInPath.Count - 1)
			{
				DicomAttributeSQ sequence = (DicomAttributeSQ)result[atrributePath.TagsInPath[i++]];
				if (sequence.IsEmpty)
					sequence.AddSequenceItem(new DicomSequenceItem());

				result = ((DicomSequenceItem[])sequence.Values)[0];
			}

			return result[atrributePath.TagsInPath[i]];
		}
	}
}
