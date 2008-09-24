using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.DataStore
{
	public partial class DataAccessLayer
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
				DicomAttributeCollection result = new DicomAttributeCollection();
				result.ValidateVrLengths = false;
				result.ValidateVrValues = false;

				foreach (QueryablePropertyInfo property in QueryableProperties<T>.GetProperties())
				{
					string criteria = _queryCriteria[property.Path];

					bool includeResult = property.IsUnique || property.IsHigherLevelUnique || criteria != null;
					if (!includeResult)
						continue;

					object propertyValue = property.ReturnProperty.GetValue(candidate, null);
					string[] testValues = new string[]{};
					//TODO: this is incorrect, it's only the criteria value that should be conditionally converted to
					//an array.  The actual property value should always be converted.
					if (property.AllowListMatching)
					{
						testValues = Convert.ToStringArray(propertyValue, property.ReturnPropertyConverter);
					}
					else
					{
						string testValue = Convert.ToDicomStringArray(propertyValue, property.ReturnPropertyConverter);
						if (testValue != null)
							testValues = new string[] { testValue };
					}

					if (criteria == null)
						criteria = "";

					//special case, we post-filter modalities in study when it contains wildcards b/c the hql query won't 
					//always produce exactly the right results.  This will never happen anyway.

					bool query = !String.IsNullOrEmpty(criteria) && ((!property.IsHigherLevelUnique && property.PostFilterOnly) || 
									(property.Path.Equals(DicomTags.ModalitiesInStudy) && ContainsWildCharacters(criteria)));

					if (query)
					{
						string[] criteriaValues;
						if (property.AllowListMatching)
							criteriaValues = DicomStringHelper.GetStringArray(criteria);
						else
							criteriaValues = new string[] { criteria };

						//When something doesn't match, the candidate is not a match, and the result is filtered out.
						if (!AnyMatch(property, criteriaValues, testValues))
							return null;
					}

					string resultValue = DicomStringHelper.GetDicomStringArray(testValues);
					AddValueToResult(property.Path, resultValue, result);
				}

				return result;
			}

			private static bool AnyMatch(QueryablePropertyInfo property, IEnumerable<string> criteriaValues, IEnumerable<string> testValues)
			{
				bool testsPerformed = false;

				foreach (string criteria in criteriaValues)
				{
					if (String.IsNullOrEmpty(criteria))
						continue;

					foreach (string test in testValues)
					{
						if (String.IsNullOrEmpty(test))
							continue;

						if (property.Path.ValueRepresentation == DicomVr.UIvr) //list of uid matching.
						{
							testsPerformed = true;
							//any matching uid is considered a match.
							if (test.Equals(criteria))
								return true;
						}
						else if (property.Path.ValueRepresentation == DicomVr.DAvr)
						{
							//The raw Patient's Birth Date is in the database, and we could post-filter it, but it's optional,
							//so we'll leave it for now. The only other date/time value we support querying on is Study Date, 
							//which is done in Hql.
							return true;
						}
						else if (property.Path.ValueRepresentation == DicomVr.TMvr)
						{
							//TODO: to be totally compliant, we should be post-filtering on Study Time (it's in the database, but in raw form).
							return true;
						}
						else if (property.Path.ValueRepresentation == DicomVr.DTvr)
						{
							//We currently don't post-filter on this VR, so it's 'optional' according to Dicom and is a match.
							return true;
						}
						else if (ContainsWildCharacters(criteria)) // wildcard matching.
						{
							//Note: wildcard matching is supposed to be case-sensitive (except for PN) according to Dicom.
							//However, in practice, it's a pain for the user; for example, when searching by Study Description.

							testsPerformed = true;
							string criteriaTest = criteria.Replace("*", "[\x21-\x7E]").Replace("?", ".");
							//a match on any of the values is considered a match.
							if (Regex.IsMatch(test, criteriaTest, RegexOptions.IgnoreCase))
								return true;
						}
						else
						{
							//Note: single value matching is supposed to be case-sensitive (except for PN) according to Dicom.
							//TODO: make it always case-insensitive, to be consistent with wildcard.
							testsPerformed = true;
							if (criteria.Equals(test)) //single value matching.
								return true;
						}
					}
				}

				//if no tests were performed, then it's considered a match.
				return !testsPerformed;
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
}