using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="DateFormatApplicationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DateFormatApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DateFormatApplicationComponent class
	/// </summary>
	[AssociateView(typeof(DateFormatApplicationComponentViewExtensionPoint))]
	public class DateFormatApplicationComponent : ApplicationComponent
	{
		private DateFormatConfiguration _configuration;

		public DateFormatApplicationComponent(DateFormatConfiguration configuration)
		{
			DateFormatConfiguration.Validate(configuration);

			_configuration = configuration;
		}

		public DateFormatConfiguration Configuration
		{
			get { return _configuration; }
		}

		public string UnchangedDateFormat
		{
			get { return _configuration.UnchangedDateFormat; }
		}

		public string[] AvailableCustomDateFormats
		{
			get { return _configuration.AvailableCustomDateFormats; }
		}

		public bool UseSystemShortDate
		{
			get { return (_configuration.DateFormatSelection == DateFormatConfiguration.DateFormatSelections.SYSTEMSHORT); }
			set { SetDateFormatSelection(DateFormatConfiguration.DateFormatSelections.SYSTEMSHORT, value); }
		}

		public bool UseSystemLongDate
		{
			get { return (_configuration.DateFormatSelection == DateFormatConfiguration.DateFormatSelections.SYSTEMLONG); }
			set { SetDateFormatSelection(DateFormatConfiguration.DateFormatSelections.SYSTEMLONG, value); }
		}

		public bool UseUnchangedDate
		{
			get { return (_configuration.DateFormatSelection == DateFormatConfiguration.DateFormatSelections.UNCHANGED); }
			set { SetDateFormatSelection(DateFormatConfiguration.DateFormatSelections.UNCHANGED, value); }
		}

		public bool UseCustomDate
		{
			get { return (_configuration.DateFormatSelection == DateFormatConfiguration.DateFormatSelections.CUSTOM); }
			set { SetDateFormatSelection(DateFormatConfiguration.DateFormatSelections.CUSTOM, value); }
		}

		public string SelectedCustomDateFormat
		{
			get { return _configuration.SelectedCustomDateFormat; }
			set 
			{
				if (_configuration.SelectedCustomDateFormat != value)
					this.Modified = true;

				_configuration.SelectedCustomDateFormat = value;
			}
		}

		public int SelectedCustomDateFormatIndex
		{
			get { return _configuration.SelectedCustomDateFormatIndex; }
			set 
			{
				if (_configuration.SelectedCustomDateFormatIndex != value)
					this.Modified = true;

				_configuration.SelectedCustomDateFormatIndex = value;
			}
		}

		protected void SetDateFormatSelection(DateFormatConfiguration.DateFormatSelections selection, bool value)
		{
			if (value != (_configuration.DateFormatSelection == selection))
				this.Modified = true;

			if (value)
				_configuration.DateFormatSelection = selection;

		}

		public string SampleDate
		{
			get
			{
				if (UseCustomDate)
					return DateTime.Now.ToString(SelectedCustomDateFormat);
				if (UseUnchangedDate)
					return DateTime.Now.ToString(UnchangedDateFormat);
				if (UseSystemLongDate)
					return DateTime.Now.ToLongDateString();

				return DateTime.Now.ToShortDateString();
			}
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}
	}
}
