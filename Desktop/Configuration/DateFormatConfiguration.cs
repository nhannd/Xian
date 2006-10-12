using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.Desktop.Configuration
{
	public class DateFormatConfiguration
	{
		public enum DateFormatSelections { SYSTEMSHORT, SYSTEMLONG, UNCHANGED, CUSTOM };

		//Dicom date format.
		private string _unchangedDateFormatString = "yyyyMMdd";

		//some standard date formats.
		private string[] _availableCustomDateFormats = 
		{
            "dddd, MMMM dd, yyyy",
            "ddd, MMM d \"'\"yy",
            "dd'/'MM'/'yy",
			"dd'/'MM'/'yyyy",
			"dd-MM-yy",
			"dd-MM-yyyy",
            "yy'/'MM'/'dd",
			"yyyy'/'MM'/'dd",
			"yy-MM-dd",
			"yyyy-MM-dd"
		};

		private DateFormatSelections _dateFormatSelection = DateFormatSelections.SYSTEMSHORT;
		private int _selectedCustomDateFormatIndex = 0;

		public string UnchangedDateFormat
		{
			get { return _unchangedDateFormatString; }
			protected set { _unchangedDateFormatString = value; }
		}

		public string[] AvailableCustomDateFormats
		{
			get { return _availableCustomDateFormats; }
			protected set { _availableCustomDateFormats = value; }
		}

		public DateFormatConfiguration.DateFormatSelections DateFormatSelection
		{
			get { return _dateFormatSelection; }
			set { _dateFormatSelection = value; }
		}

		public int SelectedCustomDateFormatIndex
		{
			get { return _selectedCustomDateFormatIndex; }
			set
			{
				value = Math.Min(value, AvailableCustomDateFormats.Length - 1);
				value = Math.Max(value, 0);

				_selectedCustomDateFormatIndex = value;
			}
		}

		public string SelectedCustomDateFormat
		{
			get { return AvailableCustomDateFormats[_selectedCustomDateFormatIndex]; }
			set
			{
				_selectedCustomDateFormatIndex = 0;

				int newIndex = 0;
				foreach (string format in AvailableCustomDateFormats)
				{
					if (format == value)
					{
						_selectedCustomDateFormatIndex = newIndex;
						return;
					}

					++newIndex;
				}
			}
		}

		public string DateFormat
		{
			get
			{
				if (DateFormatSelection == DateFormatConfiguration.DateFormatSelections.SYSTEMLONG)
				{
					return Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern;
				}

				if (DateFormatSelection == DateFormatConfiguration.DateFormatSelections.UNCHANGED)
				{
					return UnchangedDateFormat;
				}

				if (DateFormatSelection == DateFormatConfiguration.DateFormatSelections.CUSTOM)
				{
					IEnumerable<string> formats = AvailableCustomDateFormats;
					foreach (string format in formats)
					{
						if (format == SelectedCustomDateFormat)
							return format;
					}
				}

				//by default, return the system short date pattern.
				return Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
			}
		}

		public static void Validate(DateFormatConfiguration configuration)
		{
			Platform.CheckForNullReference(configuration, "configuration");

			Platform.CheckForNullReference(configuration.UnchangedDateFormat, "configuration.UnchangedDateFormat");
			Platform.CheckForEmptyString(configuration.UnchangedDateFormat, "configuration.UnchangedDateFormat");

			Platform.CheckForNullReference(configuration.AvailableCustomDateFormats, "configuration.AvailableCustomDateFormats");
			Platform.CheckArgumentRange(configuration.AvailableCustomDateFormats.Length, 1, int.MaxValue, "configuration.AvailableCustomDateFormats");
		}
	}
}
