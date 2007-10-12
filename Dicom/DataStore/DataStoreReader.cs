#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate;
using NHibernate.Expression;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private class DataStoreReader : SessionConsumer, IDataStoreReader
		{
			public DataStoreReader(ISessionManager sessionManager)
				: base(sessionManager)
			{
			}

			private static bool StudyExists(ISession session, Uid referenceUid)
			{
				string queryString = "select count(study) from Study study where study.StudyInstanceUid = ?";
				IEnumerable results = session.Enumerable(queryString, referenceUid.ToString(), NHibernateUtil.String);

				foreach (int number in results)
					return number > 0;

				return false;
			}

			private static bool SeriesExists(ISession session, Uid referenceUid)
			{
				string queryString = "select count(series) from Series series where series.SeriesInstanceUid = ?";
				IEnumerable results = session.Enumerable(queryString, referenceUid.ToString(), NHibernateUtil.String);

				foreach (int number in results)
					return number > 0;

				return false;
			}

			private static bool SopInstanceExists(ISession session, Uid referenceUid)
			{
				string queryString = "select count(sop) from SopInstance sop where sop.SopInstanceUid = ?";
				IEnumerable results = session.Enumerable(queryString, referenceUid.ToString(), NHibernateUtil.String);

				foreach (int number in results)
					return number > 0;

				return false;
			}

			#region IDataStore Members

			public bool StudyExists(Uid referenceUid)
			{
				try
				{
					using (SessionManager.GetReadTransaction())
					{
						return StudyExists(Session, referenceUid);
					}
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatStudyExistsFailed, referenceUid);
					throw new DataStoreException(message, e);
				}
			}

			public bool SeriesExists(Uid referenceUid)
			{
				try
				{
					using (SessionManager.GetReadTransaction())
					{
						return SeriesExists(Session, referenceUid);
					}
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatSeriesExistsFailed, referenceUid);
					throw new DataStoreException(message, e);
				}
			}

			public bool SopInstanceExists(Uid referenceUid)
			{
				try
				{
					using (SessionManager.GetReadTransaction())
					{
						return SopInstanceExists(Session, referenceUid);
					}
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatSopExistsFailed, referenceUid);
					throw new DataStoreException(message, e);
				}
			}

			public IStudy GetStudy(Uid referenceUid)
			{
				try
				{
					using (SessionManager.GetReadTransaction())
					{
						if (!StudyExists(Session, referenceUid))
							return null;

						IList listOfStudies = Session.CreateCriteria(typeof(Study))
							.Add(Expression.Eq("StudyInstanceUid", referenceUid.ToString()))
							.SetFetchMode("Series", FetchMode.Eager)
							.List();

						if (null != listOfStudies && listOfStudies.Count > 0)
						{
							Study study = (Study)listOfStudies[0];
							study.InitializeAssociatedCollection += InitializeAssociatedObject;

							foreach (Series series in study.Series)
								series.InitializeAssociatedCollection += InitializeAssociatedObject;

							return study;
						}

						return null;
					}
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatFailedToRetrieveStudy, referenceUid);
					throw new DataStoreException(message, e);
				}
			}

			public ISeries GetSeries(Uid referenceUid)
			{
				try
				{
					using (SessionManager.GetReadTransaction())
					{
						if (!SeriesExists(Session, referenceUid))
							return null;

						IList listOfSeries = Session.CreateCriteria(typeof(Series))
							.Add(Expression.Eq("SeriesInstanceUid", referenceUid.ToString()))
							.SetFetchMode("SopInstances", FetchMode.Lazy)
							.List();

						if (null != listOfSeries && listOfSeries.Count > 0)
						{
							Series series = (Series)listOfSeries[0];
							series.InitializeAssociatedCollection += InitializeAssociatedObject;
							return series;
						}

						return null;
					}
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatFailedToRetrieveSeries, referenceUid);
					throw new DataStoreException(message, e);
				}
			}

			public ISopInstance GetSopInstance(Uid referenceUid)
			{
				try
				{
					using (SessionManager.GetReadTransaction())
					{
						if (!SopInstanceExists(Session, referenceUid))
							return null;

						IList listOfSops = Session.CreateCriteria(typeof(SopInstance))
							.Add(Expression.Eq("SopInstanceUid", referenceUid.ToString()))
							.SetFetchMode("WindowValues_", FetchMode.Eager)
							.List();

						if (null != listOfSops && listOfSops.Count > 0)
							return (ISopInstance)listOfSops[0];

						return null;
					}
				}
				catch (Exception e)
				{
					string message = String.Format(SR.ExceptionFormatFailedToRetrieveSop, referenceUid);
					throw new DataStoreException(message, e);
				}
			}

			public IEnumerable<IStudy> GetStudies()
			{
				StringBuilder selectCommandString = new StringBuilder(1024);
				selectCommandString.AppendFormat("FROM Study ORDER BY StoreTime_ ");

				IList studiesFound = null;

				try
				{
					using (SessionManager.GetReadTransaction())
					{
						IQuery query = Session.CreateQuery(selectCommandString.ToString());
						studiesFound = query.List();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToRetrieveAllStudies, e);
				}

				if (studiesFound == null)
				{
					yield break;
				}
				else
				{
					foreach (Study study in studiesFound)
					{
						study.InitializeAssociatedCollection += InitializeAssociatedObject;
						foreach (Series series in study.Series)
							series.InitializeAssociatedCollection += InitializeAssociatedObject;

						yield return study;
					}
				}
			}

			public ReadOnlyQueryResultCollection StudyQuery(QueryKey queryKey)
			{
				Platform.CheckForNullReference(queryKey, "queryKey");

				StringBuilder selectCommandString = new StringBuilder(1024);
				selectCommandString.AppendFormat(" FROM Study as study ");

				bool whereClauseAdded = false;

				IDicomDictionary queryDictionary = DataAccessLayer.GetIDicomDictionary(DicomDictionary.DefaultQueryDictionaryName);

				foreach (DicomTagPath path in queryKey.DicomTagPathCollection)
				{
					if (queryKey[path].Length > 0)
					{
						//Unsupported query keys are just ignored.
						if (!queryDictionary.Contains(path))
							continue;

						DictionaryEntry column = queryDictionary.GetColumn(path);

						if (column.IsComputed)
							continue;

						StringBuilder nextCriteria = new StringBuilder();
						if (path.Equals(DicomTags.ModalitiesInStudy))
						{
							//special case for modalities in study since it's not actually in the study table.
							AppendModalitiesInStudyQuery(queryKey[DicomTags.ModalitiesInStudy], nextCriteria);
						}
						else
						{
							AppendQuery(queryKey[path], column, nextCriteria);
						}

						if (nextCriteria.Length == 0)
							continue;

						if (whereClauseAdded)
						{
							selectCommandString.AppendFormat(" AND ");
						}
						else
						{
							selectCommandString.AppendFormat("WHERE ");
							whereClauseAdded = true;
						}

						selectCommandString.Append(nextCriteria.ToString());
					}
				}

				//
				// submit the HQL query
				//
				IList studiesFound = null;
				try
				{
					using (IReadTransaction transaction = SessionManager.GetReadTransaction())
					{
						IQuery query = Session.CreateQuery(selectCommandString.ToString());
						studiesFound = query.List();
					}
				}
				catch (Exception e)
				{
					throw new DataStoreException(SR.ExceptionFailedToPerformStudyQuery, e);
				}

				return CompileResults(queryKey, studiesFound);
			}

			private ReadOnlyQueryResultCollection CompileResults(QueryKey queryKey, IList studiesFound)
			{
				QueryResultList results = new QueryResultList();

				foreach (object element in studiesFound)
				{
					Study study = (Study)element;
					QueryResult result = new QueryResult();

					IDicomDictionary resultsDictionary = DataAccessLayer.GetIDicomDictionary(DicomDictionary.DefaultResultsDictionaryName);

					foreach (PropertyInfo pi in study.GetType().GetProperties())
					{
						string fieldName = pi.Name;
						if (resultsDictionary.Contains(new TagName(fieldName)))
						{
							// ensure that property actually has a value
							object fieldValue = pi.GetValue(study, null);
							if (null == fieldValue)
								continue;

							DictionaryEntry col = resultsDictionary.GetColumn(new TagName(fieldName));
							result.Add(col.Path, fieldValue.ToString());
						}
					}

					Dictionary<string, string> setModalities = new Dictionary<string, string>();
					foreach (Series series in study.Series)
						setModalities[series.Modality] = series.Modality;

					string modalities = DicomStringHelper.GetDicomStringArray<string>(setModalities.Keys);
					result.Add(DicomTags.ModalitiesInStudy, modalities);

					results.Add(result);
				}

				return new ReadOnlyQueryResultCollection(results);
			}

			private void AppendQuery(string queryKeyString, DictionaryEntry column, StringBuilder returnBuilder)
			{
				string newQueryKeyString = StandardizeQueryKey(queryKeyString);

				if (column.ValueRepresentation == "UI")
				{
					AppendListOfUidQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
				}
				else if (column.ValueRepresentation == "DA")
				{
					AppendDateQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
				}
				else if (column.ValueRepresentation == "TM")
				{
					// Unsupported at this time.
					//AppendTimeQuery(...)
				}
				else if (column.ValueRepresentation == "DT")
				{
					// Unsupported at this time.
					//AppendDateTimeQuery(...)
				}
				else if (IsWildCardQuery(newQueryKeyString, column))
				{
					AppendWildCardQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
				}
				else
				{
					AppendSingleValueQuery(newQueryKeyString, column.TagName.ToString(), returnBuilder);
				}
			}

			private void AppendModalitiesInStudyQuery(string modalitiesInStudyQuery, StringBuilder returnBuilder)
			{
				if (String.IsNullOrEmpty(modalitiesInStudyQuery))
					return;

				if (ContainsWildCards(modalitiesInStudyQuery))
				{
					returnBuilder.AppendFormat("exists(from study.InternalSeries as series where series.Modality like '{0}')", modalitiesInStudyQuery);
				}
				else
				{
					List<string> modalities = new List<string>(DicomStringHelper.GetStringArray(modalitiesInStudyQuery));
					string modalitiesList = StringUtilities.Combine<string>(modalities, ", ", delegate(string value) { return String.Format("'{0}'", value); });
					returnBuilder.AppendFormat("exists(from study.InternalSeries as series where series.Modality in ( {0} ))", modalitiesList);
				}
			}

			private void AppendListOfUidQuery(string listOfUidQueryString, string columnName, StringBuilder returnBuilder)
			{
				if (String.IsNullOrEmpty(listOfUidQueryString))
					return;

				List<string> uids = new List<string>(DicomStringHelper.GetStringArray(listOfUidQueryString));
				string uidList = StringUtilities.Combine<string>(uids, ", ", delegate(string value) { return String.Format("'{0}'", value); });
				returnBuilder.AppendFormat("{0} in ( {1} )", columnName + "_", uidList);
			}

			private void AppendDateQuery(string dateQueryString, string columnName, StringBuilder returnBuilder)
			{
				string fromDate, toDate;
				bool isRange;

				try
				{
					DateRangeHelper.Parse(dateQueryString, out fromDate, out toDate, out isRange);
				}
				catch (Exception e)
				{
					throw;
				}

				StringBuilder dateRangeQueryBuilder = new StringBuilder();

				if (fromDate != "")
				{
					//When a dicom date is specified with no '-', it is to be taken as an exact date.
					if (!isRange)
					{
						dateRangeQueryBuilder.AppendFormat("( {0} = '{1}' )", columnName + "_", fromDate.ToString());
					}
					else
					{
						dateRangeQueryBuilder.AppendFormat("( {0} IS NOT NULL AND {0} >= '{1}' )", columnName + "_", fromDate);
					}
				}

				if (toDate != "")
				{
					if (fromDate == "")
						dateRangeQueryBuilder.AppendFormat("( {0} IS NULL OR ", columnName + "_");
					else
						dateRangeQueryBuilder.AppendFormat(" AND (");

					dateRangeQueryBuilder.AppendFormat("{0} <= '{1}' )", columnName + "_", toDate);
				}

				returnBuilder.AppendFormat("({0})", dateRangeQueryBuilder.ToString());
			}

			private void AppendWildCardQuery(string wildCardQuery, string columnName, StringBuilder returnBuilder)
			{
				string newWildCardQuery = wildCardQuery.Replace("*", "%");

				// don't forget to append the trailing underscore for column names
				returnBuilder.AppendFormat("{0} LIKE '{1}'", columnName + "_", newWildCardQuery);
			}

			private void AppendSingleValueQuery(string exactQueryString, string columnName, StringBuilder returnBuilder)
			{
				returnBuilder.AppendFormat("{0} = '{1}'", columnName + "_", exactQueryString);
			}

			private bool IsWildCardQuery(string queryStringCandidate, DictionaryEntry column)
			{
				string[] excludeVRs = { "DA", "TM", "DT", "SL", "SS", "US", "UL", "FL", "FD", "OB", "OW", "UN", "AT", "DS", "IS", "AS", "UI" };
				foreach (string excludeVR in excludeVRs)
				{
					if (0 == String.Compare(excludeVR, column.ValueRepresentation, true))
						return false;
				}

				return ContainsWildCards(queryStringCandidate);
			}

			private bool ContainsWildCards(string queryString)
			{
				string wildcardChars = "?*";

				if (queryString.IndexOfAny(wildcardChars.ToCharArray()) < 0)
					return false;

				return true;
			}

			private string StandardizeQueryKey(string inputQueryKeyString)
			{
				return inputQueryKeyString.Replace("'", "''");
			}

			#endregion
		}
	}
}