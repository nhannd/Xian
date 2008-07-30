using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.DicomServices;

namespace ClearCanvas.Dicom.DataStore
{
	internal static class QueryBuilder
	{
		private static readonly char[] WildcardChars = { '?', '*' };

		private static readonly string[] WildcardExcludedVRs = 
			{ "DA", "TM", "DT", "SL", "SS", "US", "UL", "FL", "FD", "OB", "OW", "UN", "AT", "DS", "IS", "AS", "UI" };

		public static string BuildHqlQuery<T>(QueryCriteria queryCriteria)
		{
			QueryablePropertyCollection<T> queryableProperties = new QueryablePropertyCollection<T>();

			StringBuilder hqlQuery = new StringBuilder(1024);
			hqlQuery.AppendFormat(" FROM {0} ", typeof(T).Name);

			bool whereClauseAdded = false;

			foreach (KeyValuePair<DicomTagPath, string> criteria in queryCriteria)
			{
				if (criteria.Value.Length > 0)
				{
					QueryablePropertyInfo property = queryableProperties[criteria.Key];
					//Unsupported query keys are just ignored.
					if (property == null || property.IsComputed || property.ReturnOnly)
						continue;

					string hqlCriteria = ConvertCriteria(criteria.Value, property);
					if (hqlCriteria.Length == 0)
						continue;

					if (whereClauseAdded)
					{
						hqlQuery.AppendFormat(" AND ");
					}
					else
					{
						hqlQuery.AppendFormat("WHERE ");
						whereClauseAdded = true;
					}

					hqlQuery.Append(hqlCriteria);
				}
			}

			return hqlQuery.ToString();
		}

		private static string ConvertCriteria(string criteria, QueryablePropertyInfo property)
		{
			if (String.IsNullOrEmpty(criteria)) //Universal matching.
				return "";

			criteria = StandardizeCriteria(criteria);

			if (property.Path.ValueRepresentation.Name == "UI")
			{
				return ConvertListOfUidCriteria(criteria, property.ColumnName);
			}
			else if (property.Path.ValueRepresentation.Name == "DA")
			{
				return ConvertDateCriteria(criteria, property.ColumnName);
			}
			else if (property.Path.ValueRepresentation.Name == "TM")
			{
				// Unsupported at this time.
				//ConvertTimeCriteria(...)
			}
			else if (property.Path.ValueRepresentation.Name == "DT")
			{
				// Unsupported at this time.
				//ConvertDateTimeCriteria(...)
			}
			else if (IsWildCardCriteria(criteria, property))
			{
				return ConvertWildCardCriteria(criteria, property.ColumnName);
			}
			else
			{
				return ConvertSingleValueCriteria(criteria, property.ColumnName);
			}

			return "";
		}

		private static string ConvertListOfUidCriteria(string listOfUidCriteria, string columnName)
		{
			string uids = StringUtilities.Combine(DicomStringHelper.GetStringArray(listOfUidCriteria), ", ", 
				delegate(string value) { return String.Format("'{0}'", value); });

			return String.Format("{0} in ( {1} )", columnName, uids);
		}

		private static string ConvertDateCriteria(string dateCriteria, string columnName)
		{
			string fromDate, toDate;
			bool isRange;

			DateRangeHelper.Parse(dateCriteria, out fromDate, out toDate, out isRange);
			StringBuilder dateRangeCriteria = new StringBuilder();

			if (fromDate != "")
			{
				//When a dicom date is specified with no '-', it is to be taken as an exact date.
				if (!isRange)
				{
					dateRangeCriteria.AppendFormat("( {0} = '{1}' )", columnName, fromDate);
				}
				else
				{
					dateRangeCriteria.AppendFormat("( {0} IS NOT NULL AND {0} >= '{1}' )", columnName, fromDate);
				}
			}

			if (toDate != "")
			{
				if (fromDate == "")
					dateRangeCriteria.AppendFormat("( {0} IS NULL OR ", columnName);
				else
					dateRangeCriteria.AppendFormat(" AND (");

				dateRangeCriteria.AppendFormat("{0} <= '{1}' )", columnName, toDate);
			}

			//will only happen if the query string is bad.
			string returnCriteria = dateRangeCriteria.ToString();
			if (String.IsNullOrEmpty(returnCriteria))
				return returnCriteria;

			return String.Format("({0})", returnCriteria);
		}

		private static string ConvertWildCardCriteria(string wildCardCriteria, string columnName)
		{
			wildCardCriteria = wildCardCriteria.Replace("*", "%");

			// don't forget to append the trailing underscore for column names
			return String.Format("{0} LIKE '{1}'", columnName, wildCardCriteria);
		}

		private static string ConvertSingleValueCriteria(string singleValueCriteria, string columnName)
		{
			return String.Format("{0} = '{1}'", columnName, singleValueCriteria);
		}

		private static bool IsWildCardCriteria(string criteria, QueryablePropertyInfo column)
		{
			foreach (string excludeVR in WildcardExcludedVRs)
			{
				if (0 == String.Compare(excludeVR, column.Path.ValueRepresentation.Name, true))
					return false;
			}

			if (criteria.IndexOfAny(WildcardChars) < 0)
				return false;

			return true;
		}

		private static string StandardizeCriteria(string criteria)
		{
			return criteria.Replace("'", "''");
		}
	}
}
