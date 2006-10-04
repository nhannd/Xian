using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomConfigurationApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DateFormatApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DicomConfigurationApplicationComponent class
	/// </summary>
	[AssociateView(typeof(DateFormatApplicationComponentViewExtensionPoint))]
	public class DateFormatApplicationComponent : ApplicationComponent
	{
		private enum DateFormatSelections { SYSTEMSHORT, SYSTEMLONG, UNCHANGED, CUSTOM };
		
		//some standard date formats.
		private string[] _availableCustomDateFormats = 
		{
            "dddd, MMMM dd yyyy",
            "ddd, MMM d \"'\"yy",
            "dddd, MMMM dd",
            "dd'/'MM'/'yy",
			"dd'/'MM'/'yyyy",
			"dd-MM-yy",
			"dd-MM-yyyy",
            "yy'/'MM'/'dd",
			"yyyy'/'MM'/'dd",
			"yy-MM-dd",
			"yyyy-MM-dd"
		};

		private string _unchangedFormatString = "yyyyMMdd";
		private int _selectedCustomDateFormatIndex;
			
		DateFormatSelections _dateFormatSelection = DateFormatSelections.SYSTEMSHORT;
		
		public bool UseSystemShortDate
		{
			get { return _dateFormatSelection == DateFormatSelections.SYSTEMSHORT; }
			set 
			{
				if (value)
					_dateFormatSelection = DateFormatSelections.SYSTEMSHORT;
			}
		}

		public bool UseSystemLongDate
		{
			get { return _dateFormatSelection == DateFormatSelections.SYSTEMLONG; }
			set
			{
				if (value)
					_dateFormatSelection = DateFormatSelections.SYSTEMLONG;
			}
		}

		public bool UseUnchangedDate
		{
			get { return _dateFormatSelection == DateFormatSelections.UNCHANGED; }
			set
			{
				if (value)
					_dateFormatSelection = DateFormatSelections.UNCHANGED;
			}
		}

		public bool UseCustomDate
		{
			get { return _dateFormatSelection == DateFormatSelections.CUSTOM; }
			set
			{
				if (value)
					_dateFormatSelection = DateFormatSelections.CUSTOM;
			}
		}

		public string[] AvailableCustomDateFormats
		{
			get { return _availableCustomDateFormats; }
			set 
			{
				if (value == null || value.Length == 0)
					return;

				_availableCustomDateFormats = value; 
			}
		}

		public int SelectedCustomDateFormatIndex
		{
			get { return _selectedCustomDateFormatIndex; }
			set
			{
				value = Math.Min(value, _availableCustomDateFormats.Length - 1);
				value = Math.Max(value, 0);

				_selectedCustomDateFormatIndex = value;
			}
		}

		public string SelectedCustomDateFormat
		{
			get { return _availableCustomDateFormats[_selectedCustomDateFormatIndex]; }
			set
			{
				int i = 0;
				foreach (string format in _availableCustomDateFormats)
				{
					if (format == value)
					{
						_selectedCustomDateFormatIndex = i;
						break;
					}
					++i;
				}
			}
		}

		public string UnchangedDateFormat
		{
			get { return _unchangedFormatString; }
			set { _unchangedFormatString = value; }
		}

		public string SampleDate
		{
			get
			{
				if (UseCustomDate)
					return DateTime.Now.ToString(SelectedCustomDateFormat);
				if (UseUnchangedDate)
					return DateTime.Now.ToString(_unchangedFormatString);
				if (UseSystemLongDate)
					return DateTime.Now.ToLongDateString();

				return DateTime.Now.ToShortDateString();
			}
		}

		protected virtual void LoadConfiguration()
		{
		}

		protected virtual void SaveConfiguration()
		{
		}

		/// <summary>
		/// </summary>
		public DateFormatApplicationComponent()
		{
			SelectedCustomDateFormatIndex = 0;
		}

		public override void Start()
		{
			LoadConfiguration();
			base.Start();
		}

		public override void Stop()
		{
			if (ExitCode == ApplicationComponentExitCode.Normal)
				SaveConfiguration();

			base.Stop();
		}
	}
}
