using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Will parse a date range adhering to the dicom format.  For example:
	/// 
	///		DateRange				From (parsed)		To (parsed)			Range?
	/// ----------------------------------------------------------------------------
	///		20070606				20070606			-					No
	/// 	20070606-				20070606			-					Yes
	/// 	-20070606				Beginning of time	20070606			Yes
	///		20060101-20070606		20060101			20070606			Yes
	/// </summary>
	public class DateRangeParser
	{
		/// <summary>
		/// Will parse a date range adhering to the dicom date format, returning the dates as <see cref="DateTime"/> objects.
		/// </summary>
		/// <param name="dateRange">the string to be parsed</param>
		/// <param name="fromDate">the "from date", or null</param>
		/// <param name="toDate">the "to date" or null</param>
		/// <param name="isRange">whether or not the input value was actually a range.  If not, then the "from date" value should be taken 
		/// to be an exact value, not a range, depending on the application.</param>
		/// <exception cref="InvalidOperationException">if the input range is poorly formatted</exception>
		public static void Parse(string dateRange, out DateTime? fromDate, out DateTime? toDate, out bool isRange)
		{
			try
			{
				fromDate = null;
				toDate = null;
				isRange = false;

				if (dateRange == null)
					return;

				string fromDateString = "", toDateString = "";
				string[] splitRange = dateRange.Split('-');

				if (splitRange.Length == 1)
				{
					fromDateString = splitRange[0];
				}
				else if (splitRange.Length == 2)
				{
					fromDateString = splitRange[0];
					toDateString = splitRange[1];
					isRange = true;
				}
				else
				{
					throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));
				}

				DateTime outDate;

				if (fromDateString == "")
				{
					fromDate = null;
				}
				else
				{
					if (!DateParser.Parse(fromDateString, out outDate))
						throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));

					fromDate = outDate;
				}


				if (toDateString == "")
				{
					toDate = null;
				}
				else
				{
					if (!DateParser.Parse(toDateString, out outDate))
						throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));

					toDate = outDate;
				}

				if (fromDate != null && toDate != null)
				{
					if (fromDate > toDate)
						throw new InvalidOperationException(string.Format(SR.ExceptionPoorlyFormattedDateRange, dateRange));
				}
			}
			catch
			{
				fromDate = toDate = null;
				throw;
			}
		}

		/// <summary>
		/// Will parse a date range adhering to the dicom date format, returning the dates as integers.
		/// </summary>
		/// <param name="dateRange">the string to be parsed</param>
		/// <param name="fromDate">the "from date", or null</param>
		/// <param name="toDate">the "to date" or null</param>
		/// <param name="isRange">whether or not the input value was actually a range.  If not, then the "from date" value should be taken 
		/// to be an exact value, not a range, depending on the application.</param>
		public static void Parse(string dateRange, out int fromDate, out int toDate, out bool isRange)
		{
			string fromDateString, toDateString;
			Parse(dateRange, out fromDateString, out toDateString, out isRange);

			if (fromDateString == "")
			{
				fromDate = 0;
			}
			else
			{
				//the string is guaranteed to be formatted like "yyyyMMdd", so this is safe.
				fromDate = Convert.ToInt32(fromDateString);
			}

			if (toDateString == "")
			{
				toDate = 0;
			}
			else
			{
				//the string is guaranteed to be formatted like "yyyyMMdd", so this is safe.
				toDate = Convert.ToInt32(toDateString);
			}
		}

		/// <summary>
		/// Will parse a date range adhering to the dicom date format, returning the dates as strings.  In the case where the input
		/// dates are formatted according to the old Dicom Standard (yyyy.MM.dd), the resulting strings will be reformatted according
		/// to the current Dicom Standard.
		/// </summary>
		/// <param name="dateRange">the string to be parsed</param>
		/// <param name="fromDate">the "from date", or null</param>
		/// <param name="toDate">the "to date" or null</param>
		/// <param name="isRange">whether or not the input value was actually a range.  If not, then the "from date" value should be taken 
		/// to be an exact value, not a range, depending on the application.</param>
		public static void Parse(string dateRange, out string fromDate, out string toDate, out bool isRange)
		{
			DateTime? from, to;
			Parse(dateRange, out from, out to, out isRange);

			fromDate = (from != null) ? ((DateTime)from).ToString(DateParser.DicomDateFormat) : "";
			toDate = (to != null) ? ((DateTime)to).ToString(DateParser.DicomDateFormat) : "";
		}
	}
}
