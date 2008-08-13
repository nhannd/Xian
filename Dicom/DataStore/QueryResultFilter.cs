using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ClearCanvas.DicomServices;

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

					bool includeResult = criteria != null || property.IsRequired || property.IsUnique || property.IsHigherLevelUnique;
					if (!includeResult)
						continue;

					object propertyValue = property.ReturnProperty.GetValue(candidate, null);
					string[] testValues = new string[]{};
					if (property.AllowListMatching)
					{
						testValues = Convert.ToStringArray(propertyValue, property.ReturnPropertyConverter);
					}
					else
					{
						string testValue = Convert.ToString(propertyValue, property.ReturnPropertyConverter);
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
							//We currently don't post-filter on this VR, so it's 'optional' according to Dicom and is a match.

							//The only date/time value we support querying on is Study Date, which is done in Hql.
							return true;
						}
						else if (property.Path.ValueRepresentation == DicomVr.TMvr)
						{
							//We currently don't post-filter on this VR, so it's 'optional' according to Dicom and is a match.
							return true;
						}
						else if (property.Path.ValueRepresentation == DicomVr.DTvr)
						{
							//We currently don't post-filter on this VR, so it's 'optional' according to Dicom and is a match.
							return true;
						}
						else if (ContainsWildCharacters(criteria)) // wildcard matching.
						{
							testsPerformed = true;
							string criteriaTest = criteria.Replace("*", "[\x21-\x7E]").Replace("?", ".");
							//a match on any of the values is considered a match.
							if (Regex.IsMatch(test, criteriaTest, RegexOptions.IgnoreCase))
								return true;
						}
						else
						{
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